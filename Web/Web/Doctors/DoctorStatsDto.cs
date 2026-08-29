namespace Web.Doctors;

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
