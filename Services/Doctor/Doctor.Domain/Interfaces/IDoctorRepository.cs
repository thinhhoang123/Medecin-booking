using Doctor.Domain.Entities;
using Doctor.Domain.Enums;
using Doctor.Domain.ValueObjects;

namespace Doctor.Domain.Interfaces
{
    /// <summary>
    /// Doctor repository interface with specialized queries
    /// </summary>
    public interface IDoctorRepository : IRepository<Entities.Doctor>
    {
        Task<Entities.Doctor?> GetByEmailAsync(string email);
        Task<Entities.Doctor?> GetByLicenseNumberAsync(string licenseNumber);
        Task<IEnumerable<Entities.Doctor>> GetBySpecializationAsync(Specialization specialization);
        Task<IEnumerable<Entities.Doctor>> GetByDepartmentAsync(string department);

        Task<IEnumerable<Entities.Doctor>> GetAvailableDoctorsAsync(DateTime date, Specialization? specialization = null);
        Task<IEnumerable<Entities.Doctor>> GetDoctorsByWorkingHoursAsync(WorkingHours workingHours);
        Task<IEnumerable<Entities.Doctor>> GetDoctorsByTimeSlotAsync(TimeSlot timeSlot, DayOfWeek dayOfWeek);

        Task<IEnumerable<Entities.Doctor>> SearchDoctorsAsync(string searchTerm);
        Task<IEnumerable<Entities.Doctor>> SearchDoctorsAdvancedAsync(
            string? searchTerm = null,
            string? specialization = null,
            string? department = null,
            bool? isAvailable = null,
            DateTime? availableDate = null);

        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByLicenseNumberAsync(string licenseNumber);
        Task<bool> ExistsByUserIdAsync(int userId);

        Task<IEnumerable<Entities.Doctor>> GetDoctorsWithSchedulesAsync();
        Task<IEnumerable<Entities.Doctor>> GetDoctorsWithActiveSchedulesAsync();
        Task<IEnumerable<Entities.Doctor>> GetDoctorsByScheduleDayAsync(DayOfWeek dayOfWeek);
        Task<IEnumerable<Entities.Doctor>> GetDoctorsByScheduleDateRangeAsync(DateTime startDate, DateTime endDate);

        Task<int> GetActiveDoctorsCountAsync();
        Task<int> GetAvailableDoctorsCountAsync(DateTime date);
        Task<Dictionary<Specialization, int>> GetDoctorsCountBySpecializationAsync();
        Task<Dictionary<string, int>> GetDoctorsCountByDepartmentAsync();

        Task UpdateAvailabilityBulkAsync(IEnumerable<int> doctorIds, bool isAvailable);
        Task UpdateStatusBulkAsync(IEnumerable<int> doctorIds, DoctorStatus newStatus);

        Task<IEnumerable<Entities.Doctor>> GetDoctorsWithUpcomingAppointmentsAsync(DateTime date);
        Task<IEnumerable<Entities.Doctor>> GetInactiveDoctorsAsync();
        Task<IEnumerable<Entities.Doctor>> GetDoctorsWithScheduleConflictsAsync();
    }

    /// <summary>
    /// Doctor schedule repository interface
    /// </summary>
    public interface IDoctorScheduleRepository : IRepository<DoctorSchedule>
    {
        // ============ Basic Queries ============
        Task<IEnumerable<DoctorSchedule>> GetSchedulesByDoctorAsync(int doctorId);
        Task<IEnumerable<DoctorSchedule>> GetSchedulesByDoctorAndDayAsync(int doctorId, DayOfWeek day);
        Task<IEnumerable<DoctorSchedule>> GetActiveSchedulesAsync(int doctorId);
        Task<DoctorSchedule?> GetScheduleByDoctorAndDayAsync(int doctorId, DayOfWeek day);

        // ============ Date Range Queries ============
        Task<IEnumerable<DoctorSchedule>> GetSchedulesByDateRangeAsync(int doctorId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<DoctorSchedule>> GetActiveSchedulesForDateAsync(DateTime date);

        // ============ Time Slot Queries ============
        Task<IEnumerable<DoctorSchedule>> GetSchedulesByTimeSlotAsync(TimeSlot timeSlot);
        Task<IEnumerable<DoctorSchedule>> GetSchedulesByWorkingHoursAsync(WorkingHours workingHours);

        // ============ Conflict Detection ============
        Task<bool> HasScheduleConflictAsync(int doctorId, DayOfWeek day, TimeSpan startTime, TimeSpan endTime);
        Task<bool> HasScheduleConflictAsync(DoctorSchedule schedule);
        Task<IEnumerable<DoctorSchedule>> GetConflictingSchedulesAsync(int doctorId, DayOfWeek day, TimeSpan startTime, TimeSpan endTime);

        // ============ Bulk Operations ============
        Task AddSchedulesBulkAsync(IEnumerable<DoctorSchedule> schedules);
        Task ActivateSchedulesBulkAsync(IEnumerable<int> scheduleIds);
        Task DeactivateSchedulesBulkAsync(IEnumerable<int> scheduleIds);
    }
}