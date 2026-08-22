using Doctor.Domain.Enums;

namespace Doctor.Application.DTOs
{
    public class DoctorDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string SpecializationDisplay => GetDisplayName(Specialization);
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? MobileNumber { get; set; }
        public string? Address { get; set; }
        public string? Bio { get; set; }
        public string? Qualifications { get; set; }
        public string? LicenseNumber { get; set; }
        public string? Department { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsAvailableForAppointments { get; set; }
        public List<DoctorScheduleDto> Schedules { get; set; } = new();

        private string GetDisplayName(string specialization)
        {
            return Enum.TryParse<Specialization>(specialization, out var spec)
                ? spec.GetDisplayName()
                : specialization;
        }
    }

    public class CreateDoctorDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? MobileNumber { get; set; }
        public string? Address { get; set; }
        public string? Bio { get; set; }
        public string? Qualifications { get; set; }
        public string? LicenseNumber { get; set; }
        public string? Department { get; set; }
        public int? UserId { get; set; }
        public List<CreateDoctorScheduleDto>? Schedules { get; set; }
    }

    public class UpdateDoctorDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Specialization { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? MobileNumber { get; set; }
        public string? Address { get; set; }
        public string? Bio { get; set; }
        public string? Qualifications { get; set; }
        public string? Department { get; set; }
        public string? Status { get; set; }
        public bool? IsAvailableForAppointments { get; set; }
    }

    public class DoctorScheduleDto
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int SlotDurationInMinutes { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateDoctorScheduleDto
    {
        public int DoctorId { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int SlotDurationInMinutes { get; set; } = 30;
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class UpdateDoctorScheduleDto
    {
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public int? SlotDurationInMinutes { get; set; }
        public string? Status { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class DoctorAvailabilityDto
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public List<TimeSlotDto> AvailableSlots { get; set; } = new();
        public bool IsDoctorAvailable => AvailableSlots.Any();
    }

    public class TimeSlotDto
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsBooked { get; set; }
    }

    public class DoctorSearchDto
    {
        public string? SearchTerm { get; set; }
        public string? Specialization { get; set; }
        public DateTime? AvailableDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class DoctorStatsDto
    {
        public int TotalDoctors { get; set; }
        public int ActiveDoctors { get; set; }
        public int InactiveDoctors { get; set; }
        public int OnLeaveDoctors { get; set; }
        public int AvailableDoctors { get; set; }
        public Dictionary<string, int> DoctorsBySpecialization { get; set; } = new();
        public double AverageAppointmentsPerDay { get; set; }
        public Dictionary<string, int> WeeklyAvailability { get; set; } = new();
    }
}