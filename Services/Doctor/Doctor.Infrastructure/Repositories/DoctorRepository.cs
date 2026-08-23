using Doctor.Domain.Enums;
using Doctor.Domain.Interfaces;
using Doctor.Domain.ValueObjects;
using Doctor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Doctor.Infrastructure.Repositories
{
    public class DoctorRepository : BaseRepository<Domain.Entities.Doctor>, IDoctorRepository
    {
        private readonly ILogger<DoctorRepository> _logger;

        public DoctorRepository(DoctorDbContext context, ILogger<DoctorRepository> logger)
            : base(context, logger)
        {
            _logger = logger;
        }

        public async Task<Domain.Entities.Doctor?> GetByEmailAsync(string email)
        {
            try
            {
                return await _dbSet
                    .Include(d => d.Schedules)
                    .FirstOrDefaultAsync(d => d.ContactInfo.Email == email && !d.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctor by email: {Email}", email);
                throw;
            }
        }

        public async Task<Domain.Entities.Doctor?> GetByLicenseNumberAsync(string licenseNumber)
        {
            try
            {
                return await _dbSet
                    .Include(d => d.Schedules)
                    .FirstOrDefaultAsync(d => d.LicenseNumber == licenseNumber && !d.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctor by license number: {LicenseNumber}", licenseNumber);
                throw;
            }
        }

        public async Task<IEnumerable<Domain.Entities.Doctor>> GetBySpecializationAsync(Specialization specialization)
        {
            try
            {
                return await _dbSet
                    .Include(d => d.Schedules)
                    .Where(d => d.Specialization == specialization && !d.IsDeleted)
                    .OrderBy(d => d.FirstName)
                    .ThenBy(d => d.LastName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctors by specialization: {Specialization}", specialization);
                throw;
            }
        }

        public async Task<IEnumerable<Domain.Entities.Doctor>> GetByDepartmentAsync(string department)
        {
            try
            {
                return await _dbSet
                    .Include(d => d.Schedules)
                    .Where(d => d.Department == department && !d.IsDeleted)
                    .OrderBy(d => d.FirstName)
                    .ThenBy(d => d.LastName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctors by department: {Department}", department);
                throw;
            }
        }

        public async Task<IEnumerable<Domain.Entities.Doctor>> GetAvailableDoctorsAsync(DateTime date, Specialization? specialization = null)
        {
            try
            {
                var dayOfWeek = date.DayOfWeek;
                var query = _dbSet
                    .Include(d => d.Schedules)
                    .Where(d => d.Status == DoctorStatus.Active &&
                                d.IsAvailableForAppointments &&
                                !d.IsDeleted);

                if (specialization.HasValue)
                {
                    query = query.Where(d => d.Specialization == specialization.Value);
                }

                query = query.Where(d => d.Schedules.Any(s =>
                    s.DayOfWeek == dayOfWeek &&
                    s.IsActive &&
                    s.ValidFrom <= date &&
                    (!s.ValidTo.HasValue || s.ValidTo >= date)));

                return await query
                    .OrderBy(d => d.FirstName)
                    .ThenBy(d => d.LastName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available doctors for date: {Date}", date);
                throw;
            }
        }

        public Task<IEnumerable<Domain.Entities.Doctor>> GetDoctorsByWorkingHoursAsync(WorkingHours workingHours)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Domain.Entities.Doctor>> GetDoctorsByTimeSlotAsync(TimeSlot timeSlot, DayOfWeek dayOfWeek)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Domain.Entities.Doctor>> SearchDoctorsAdvancedAsync(string? searchTerm = null, string? specialization = null, string? department = null,
            bool? isAvailable = null, DateTime? availableDate = null)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            try
            {
                return await _dbSet.AnyAsync(d => d.ContactInfo.Email == email && !d.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking doctor existence by email: {Email}", email);
                throw;
            }
        }

        public Task<bool> ExistsByLicenseNumberAsync(string licenseNumber)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsByUserIdAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Domain.Entities.Doctor>> SearchDoctorsAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return await GetAllAsync();

                var lowerSearchTerm = searchTerm.ToLower();
                return await _dbSet
                    .Include(d => d.Schedules)
                    .Where(d => !d.IsDeleted &&
                        (d.FirstName.ToLower().Contains(lowerSearchTerm) ||
                         d.LastName.ToLower().Contains(lowerSearchTerm) ||
                         (d.FirstName + " " + d.LastName).ToLower().Contains(lowerSearchTerm) ||
                         d.ContactInfo.Email.ToLower().Contains(lowerSearchTerm) ||
                         d.Specialization.ToString().ToLower().Contains(lowerSearchTerm) ||
                         (d.Department != null && d.Department.ToLower().Contains(lowerSearchTerm))))
                    .OrderBy(d => d.FirstName)
                    .ThenBy(d => d.LastName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching doctors with term: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<Domain.Entities.Doctor>> GetDoctorsWithSchedulesAsync()
        {
            try
            {
                return await _dbSet
                    .Include(d => d.Schedules)
                    .Where(d => !d.IsDeleted)
                    .OrderBy(d => d.FirstName)
                    .ThenBy(d => d.LastName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctors with schedules");
                throw;
            }
        }

        public Task<IEnumerable<Domain.Entities.Doctor>> GetDoctorsWithActiveSchedulesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Domain.Entities.Doctor>> GetDoctorsByScheduleDayAsync(DayOfWeek dayOfWeek)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Domain.Entities.Doctor>> GetDoctorsByScheduleDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetActiveDoctorsCountAsync()
        {
            throw new NotImplementedException();
        }

        public Task<int> GetAvailableDoctorsCountAsync(DateTime date)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<Specialization, int>> GetDoctorsCountBySpecializationAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, int>> GetDoctorsCountByDepartmentAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateAvailabilityBulkAsync(IEnumerable<int> doctorIds, bool isAvailable)
        {
            throw new NotImplementedException();
        }

        public Task UpdateStatusBulkAsync(IEnumerable<int> doctorIds, DoctorStatus newStatus)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Domain.Entities.Doctor>> GetDoctorsWithUpcomingAppointmentsAsync(DateTime date)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Domain.Entities.Doctor>> GetInactiveDoctorsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Domain.Entities.Doctor>> GetDoctorsWithScheduleConflictsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Domain.Entities.Doctor>> GetActiveDoctorsAsync()
        {
            try
            {
                return await _dbSet
                    .Include(d => d.Schedules)
                    .Where(d => d.Status == DoctorStatus.Active && !d.IsDeleted)
                    .OrderBy(d => d.FirstName)
                    .ThenBy(d => d.LastName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active doctors");
                throw;
            }
        }

        public override async Task<Domain.Entities.Doctor?> GetByIdAsync(int id)
        {
            try
            {
                return await _dbSet
                    .Include(d => d.Schedules)
                    .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctor by id: {Id}", id);
                throw;
            }
        }
    }
}