using Doctor.Domain.Enums;
using Doctor.Domain.ValueObjects;

namespace Doctor.Domain.Entities
{
    public class DoctorSchedule : BaseEntity
    {
        public int DoctorId { get; private set; }
        public DayOfWeek DayOfWeek { get; private set; }
        public WorkingHours WorkingHours { get; private set; }
        public int SlotDurationInMinutes { get; private set; }
        public ScheduleStatus Status { get; private set; }
        public DateTime? ValidFrom { get; private set; }
        public DateTime? ValidTo { get; private set; }
        public bool IsActive => Status == ScheduleStatus.Active;

        // Navigation property
        public virtual Doctor Doctor { get; private set; }

        // Private constructor for EF Core
        private DoctorSchedule() { }

        public DoctorSchedule(
            int doctorId,
            DayOfWeek dayOfWeek,
            WorkingHours workingHours,
            int slotDurationInMinutes = 30,
            DateTime? validFrom = null,
            DateTime? validTo = null,
            string? createdBy = null)
        {
            DoctorId = doctorId;
            DayOfWeek = dayOfWeek;
            WorkingHours = workingHours ?? throw new ArgumentNullException(nameof(workingHours));
            SlotDurationInMinutes = slotDurationInMinutes;
            Status = ScheduleStatus.Active;
            ValidFrom = validFrom ?? DateTime.UtcNow.Date;
            ValidTo = validTo;

            SetCreatedInfo(createdBy ?? "System");
        }

        public bool IsTimeSlotAvailable(TimeSpan startTime, TimeSpan endTime)
        {
            if (!IsActive)
                return false;

            if (startTime < WorkingHours.StartTime || endTime > WorkingHours.EndTime)
                return false;

            if (endTime <= startTime)
                return false;

            var duration = endTime - startTime;
            if (duration.TotalMinutes != SlotDurationInMinutes)
                return false;

            return true;
        }

        public List<TimeSlot> GenerateTimeSlots()
        {
            if (!IsActive)
                return new List<TimeSlot>();

            var slots = new List<TimeSlot>();
            var currentTime = WorkingHours.StartTime;
            var endTime = WorkingHours.EndTime;

            while (currentTime.Add(TimeSpan.FromMinutes(SlotDurationInMinutes)) <= endTime)
            {
                var slotEndTime = currentTime.Add(TimeSpan.FromMinutes(SlotDurationInMinutes));
                slots.Add(new TimeSlot(currentTime, slotEndTime));
                currentTime = slotEndTime;
            }

            return slots;
        }

        public void UpdateWorkingHours(WorkingHours newWorkingHours, string? updatedBy = null)
        {
            WorkingHours = newWorkingHours ?? throw new ArgumentNullException(nameof(newWorkingHours));
            SetUpdatedInfo(updatedBy);
        }

        public void UpdateSlotDuration(int durationInMinutes, string? updatedBy = null)
        {
            if (durationInMinutes <= 0)
                throw new ArgumentException("Slot duration must be positive");

            SlotDurationInMinutes = durationInMinutes;
            SetUpdatedInfo(updatedBy);
        }

        public void Activate(string? updatedBy = null)
        {
            Status = ScheduleStatus.Active;
            SetUpdatedInfo(updatedBy);
        }

        public void Deactivate(string? updatedBy = null)
        {
            Status = ScheduleStatus.Inactive;
            SetUpdatedInfo(updatedBy);
        }

        public void SetValidityPeriod(DateTime? validFrom, DateTime? validTo, string? updatedBy = null)
        {
            if (validFrom.HasValue && validTo.HasValue && validFrom > validTo)
                throw new ArgumentException("ValidFrom date cannot be after ValidTo date");

            ValidFrom = validFrom ?? DateTime.UtcNow.Date;
            ValidTo = validTo;
            SetUpdatedInfo(updatedBy);
        }
    }
}