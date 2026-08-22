using Doctor.Application.DTOs;
using Doctor.Domain.ValueObjects;

namespace Doctor.Application.Interfaces
{
    public interface IDoctorService
    {
        // Doctor Management
        Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync();
        Task<DoctorDto?> GetDoctorByIdAsync(int id);
        Task<DoctorDto?> GetDoctorByEmailAsync(string email);
        Task<DoctorDto?> GetDoctorByLicenseNumberAsync(string licenseNumber);
        Task<IEnumerable<DoctorDto>> GetDoctorsBySpecializationAsync(string specialization);
        Task<DoctorDto> CreateDoctorAsync(CreateDoctorDto doctorDto);
        Task<DoctorDto> UpdateDoctorAsync(int id, UpdateDoctorDto doctorDto);
        Task<bool> DeleteDoctorAsync(int id);
        Task<bool> DoctorExistsAsync(int id);

        // Search & Filter
        Task<IEnumerable<DoctorDto>> SearchDoctorsAsync(string searchTerm);
        Task<IEnumerable<DoctorDto>> GetAvailableDoctorsAsync(DateTime date, string? specialization = null);

        // Schedules
        Task<IEnumerable<DoctorScheduleDto>> GetDoctorSchedulesAsync(int doctorId);
        Task<DoctorScheduleDto> AddScheduleAsync(CreateDoctorScheduleDto scheduleDto);
        Task<DoctorScheduleDto> UpdateScheduleAsync(int scheduleId, UpdateDoctorScheduleDto scheduleDto);
        Task<bool> RemoveScheduleAsync(int scheduleId);
        Task<IEnumerable<DoctorScheduleDto>> GetActiveSchedulesAsync(int doctorId);

        // Availability
        Task<IEnumerable<TimeSlot>> GetAvailableTimeSlotsAsync(int doctorId, DateTime date);
        Task<bool> CheckAvailabilityAsync(int doctorId, DateTime date, TimeSpan startTime, TimeSpan endTime);
    }
}