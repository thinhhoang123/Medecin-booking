using Doctor.Domain.Entities;
using Doctor.Domain.Enums;
using Doctor.Domain.Entities;
using Doctor.Domain.Enums;

namespace Doctor.Domain.Events
{
    public class DoctorCreatedEvent : DomainEvent
    {
        public Entities.Doctor Doctor { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Email { get; }
        public string Specialization { get; }

        public DoctorCreatedEvent(Entities.Doctor doctor) : base(doctor.Id.ToString())
        {
            Doctor = doctor;
            FirstName = doctor.FirstName;
            LastName = doctor.LastName;
            Email = doctor.ContactInfo.Email;
            Specialization = doctor.Specialization.ToString();
        }
    }

    public class DoctorUpdatedEvent : DomainEvent
    {
        public Doctor.Domain.Entities.Doctor Doctor { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Email { get; }
        public string Specialization { get; }

        public DoctorUpdatedEvent(Doctor.Domain.Entities.Doctor doctor) : base(doctor.Id.ToString())
        {
            Doctor = doctor;
            FirstName = doctor.FirstName;
            LastName = doctor.LastName;
            Email = doctor.ContactInfo.Email;
            Specialization = doctor.Specialization.ToString();
        }
    }

    public class DoctorAvailabilityChangedEvent : DomainEvent
    {
        public int DoctorId { get; }
        public bool IsAvailable { get; }
        public string FullName { get; }

        public DoctorAvailabilityChangedEvent(Doctor.Domain.Entities.Doctor doctor, bool isAvailable) : base(doctor.Id.ToString())
        {
            DoctorId = doctor.Id;
            IsAvailable = isAvailable;
            FullName = doctor.FullName;
        }
    }

    public class DoctorStatusChangedEvent : DomainEvent
    {
        public int DoctorId { get; }
        public DoctorStatus OldStatus { get; }
        public DoctorStatus NewStatus { get; }
        public string FullName { get; }

        public DoctorStatusChangedEvent(Doctor.Domain.Entities.Doctor doctor, DoctorStatus oldStatus, DoctorStatus newStatus) 
            : base(doctor.Id.ToString())
        {
            DoctorId = doctor.Id;
            OldStatus = oldStatus;
            NewStatus = newStatus;
            FullName = doctor.FullName;
        }
    }

    public class DoctorScheduleAddedEvent : DomainEvent
    {
        public int DoctorId { get; }
        public int ScheduleId { get; }
        public DayOfWeek DayOfWeek { get; }
        public TimeSpan StartTime { get; }
        public TimeSpan EndTime { get; }

        public DoctorScheduleAddedEvent(Doctor.Domain.Entities.Doctor doctor, DoctorSchedule schedule) : base(doctor.Id.ToString())
        {
            DoctorId = doctor.Id;
            ScheduleId = schedule.Id;
            DayOfWeek = schedule.DayOfWeek;
            StartTime = schedule.WorkingHours.StartTime;
            EndTime = schedule.WorkingHours.EndTime;
        }
    }

    public class DoctorScheduleRemovedEvent : DomainEvent
    {
        public int DoctorId { get; }
        public int ScheduleId { get; }
        public DayOfWeek DayOfWeek { get; }

        public DoctorScheduleRemovedEvent(Doctor.Domain.Entities.Doctor doctor, DoctorSchedule schedule) : base(doctor.Id.ToString())
        {
            DoctorId = doctor.Id;
            ScheduleId = schedule.Id;
            DayOfWeek = schedule.DayOfWeek;
        }
    }
}