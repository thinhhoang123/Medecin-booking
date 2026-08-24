namespace BlazorClient.Features.Doctors;

public class DoctorDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Specialization { get; set; } = string.Empty;
    public string SpecializationDisplay => GetSpecializationDisplay(Specialization);
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

    private string GetSpecializationDisplay(string specialization)
    {
        return specialization switch
        {
            "Cardiology" => "Cardiology",
            "Dermatology" => "Dermatology",
            "Endocrinology" => "Endocrinology",
            "Gastroenterology" => "Gastroenterology",
            "Hematology" => "Hematology",
            "InfectiousDisease" => "Infectious Disease",
            "InternalMedicine" => "Internal Medicine",
            "Nephrology" => "Nephrology",
            "Neurology" => "Neurology",
            "ObstetricsGynecology" => "Obstetrics & Gynecology",
            "Oncology" => "Oncology",
            "Ophthalmology" => "Ophthalmology",
            "Orthopedics" => "Orthopedics",
            "Otolaryngology" => "Otolaryngology",
            "Pediatrics" => "Pediatrics",
            "Pulmonology" => "Pulmonology",
            "Psychiatry" => "Psychiatry",
            "Radiology" => "Radiology",
            "Rheumatology" => "Rheumatology",
            "Surgery" => "Surgery",
            "Urology" => "Urology",
            _ => specialization
        };
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

public class TimeSlotDto
{
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsBooked { get; set; }
}

public class DoctorSearchResultDto
{
    public IEnumerable<DoctorDto> Doctors { get; set; } = Enumerable.Empty<DoctorDto>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}