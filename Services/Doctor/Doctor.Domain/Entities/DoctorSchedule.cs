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

    }

}
