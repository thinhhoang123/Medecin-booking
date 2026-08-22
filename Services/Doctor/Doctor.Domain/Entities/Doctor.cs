using Doctor.Domain.Enums;
using Doctor.Domain.ValueObjects;

namespace Doctor.Domain.Entities
{
    public class Doctor : BaseEntity
    {
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
        public virtual ICollection<DoctorSchedule> Schedules { get; private set; }

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
            Schedules = new List<DoctorSchedule>();

            // Set BaseEntity properties
            SetCreatedInfo(userId?.ToString() ?? "System");

        }

    }

}
