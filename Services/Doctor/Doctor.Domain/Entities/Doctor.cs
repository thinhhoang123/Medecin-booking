using Doctor.Domain.Enums;
using Doctor.Domain.ValueObjects;
using Doctor.Domain.Events;

namespace Doctor.Domain.Entities
{
    public class Doctor : BaseEntity
    {
        // Properties
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string FullName => $"{FirstName} {LastName}";
        public Specialization Specialization { get; private set; }
        public ContactInfo ContactInfo { get; private set; }
        public string? Bio { get; private set; }
        public string? Qualifications { get; private set; }
        public string? LicenseNumber { get; private set; }
        public string? Department { get; private set; }
        public DoctorStatus Status { get; private set; }
        public bool IsAvailableForAppointments { get; private set; }
        public int? UserId { get; private set; }

        // Navigation Properties
        private readonly List<DoctorSchedule> _schedules = new();
        public virtual IReadOnlyCollection<DoctorSchedule> Schedules => _schedules.AsReadOnly();

        // Domain Events
        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        // Private constructor for EF Core
        private Doctor() { }

        // Constructor
        public Doctor(
            string firstName,
            string lastName,
            Specialization specialization,
            ContactInfo contactInfo,
            string? bio = null,
            string? qualifications = null,
            string? licenseNumber = null,
            string? department = null,
            int? userId = null)
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            Specialization = specialization;
            ContactInfo = contactInfo ?? throw new ArgumentNullException(nameof(contactInfo));
            Bio = bio;
            Qualifications = qualifications;
            LicenseNumber = licenseNumber;
            Department = department;
            Status = DoctorStatus.Active;
            IsAvailableForAppointments = true;
            UserId = userId;

            SetCreatedInfo(userId?.ToString() ?? "System");
            AddDomainEvent(new DoctorCreatedEvent(this));
        }

        // Business Logic Methods
        public void UpdateProfile(
            string firstName,
            string lastName,
            Specialization specialization,
            ContactInfo contactInfo,
            string? bio = null,
            string? qualifications = null,
            string? department = null,
            string? updatedBy = null)
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            Specialization = specialization;
            ContactInfo = contactInfo ?? throw new ArgumentNullException(nameof(contactInfo));
            Bio = bio;
            Qualifications = qualifications;
            Department = department;

            SetUpdatedInfo(updatedBy);
            AddDomainEvent(new DoctorUpdatedEvent(this));
        }

        public void AddSchedule(DoctorSchedule schedule)
        {
            if (schedule == null)
                throw new ArgumentNullException(nameof(schedule));

            if (_schedules.Any(s => s.DayOfWeek == schedule.DayOfWeek && s.IsActive))
            {
                throw new InvalidOperationException($"Schedule already exists for {schedule.DayOfWeek}");
            }

            _schedules.Add(schedule);
            SetUpdatedInfo();
            AddDomainEvent(new DoctorScheduleAddedEvent(this, schedule));
        }

        public void RemoveSchedule(DoctorSchedule schedule)
        {
            if (schedule == null)
                throw new ArgumentNullException(nameof(schedule));

            if (!_schedules.Contains(schedule))
                throw new InvalidOperationException("Schedule not found");

            schedule.Deactivate();
            _schedules.Remove(schedule);
            SetUpdatedInfo();
            AddDomainEvent(new DoctorScheduleRemovedEvent(this, schedule));
        }

        public void UpdateAvailability(bool isAvailable, string? updatedBy = null)
        {
            if (IsAvailableForAppointments != isAvailable)
            {
                IsAvailableForAppointments = isAvailable;
                SetUpdatedInfo(updatedBy);
                AddDomainEvent(new DoctorAvailabilityChangedEvent(this, isAvailable));
            }
        }

        public void UpdateStatus(DoctorStatus newStatus, string? updatedBy = null)
        {
            if (Status != newStatus)
            {
                var oldStatus = Status;
                Status = newStatus;
                SetUpdatedInfo(updatedBy);

                if (newStatus == DoctorStatus.Inactive || newStatus == DoctorStatus.Suspended)
                {
                    IsAvailableForAppointments = false;
                }

                AddDomainEvent(new DoctorStatusChangedEvent(this, oldStatus, newStatus));
            }
        }

        public bool IsAvailableOn(DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            if (!IsAvailableForAppointments || Status != DoctorStatus.Active)
                return false;

            var dayOfWeek = date.DayOfWeek;
            var schedule = _schedules.FirstOrDefault(s =>
                s.DayOfWeek == dayOfWeek &&
                s.IsActive &&
                s.ValidFrom <= date &&
                (!s.ValidTo.HasValue || s.ValidTo >= date));

            if (schedule == null)
                return false;

            return schedule.IsTimeSlotAvailable(startTime, endTime);
        }

        public List<TimeSlot> GetAvailableTimeSlots(DateTime date)
        {
            if (!IsAvailableForAppointments || Status != DoctorStatus.Active)
                return new List<TimeSlot>();

            var dayOfWeek = date.DayOfWeek;
            var schedule = _schedules.FirstOrDefault(s =>
                s.DayOfWeek == dayOfWeek &&
                s.IsActive &&
                s.ValidFrom <= date &&
                (!s.ValidTo.HasValue || s.ValidTo >= date));

            if (schedule == null)
                return new List<TimeSlot>();

            return schedule.GenerateTimeSlots();
        }

        // Domain Events
        public void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}