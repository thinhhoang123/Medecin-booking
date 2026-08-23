using Doctor.Domain.Entities;
using Doctor.Domain.Enums;
using Doctor.Domain.Interfaces;
using Doctor.Domain.ValueObjects;
using Doctor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Doctor.Infrastructure.Repositories
{
    public class DoctorScheduleRepository : BaseRepository<DoctorSchedule>, IDoctorScheduleRepository
    {
        private readonly ILogger<DoctorScheduleRepository> _logger;

        public DoctorScheduleRepository(DoctorDbContext context, ILogger<DoctorScheduleRepository> logger)
            : base(context, logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<DoctorSchedule>> GetSchedulesByDoctorAsync(int doctorId)
        {
            try
            {
                return await _dbSet
                    .Include(s => s.Doctor)
                    .Where(s => s.DoctorId == doctorId && !s.IsDeleted)
                    .OrderBy(s => s.DayOfWeek)
                    .ThenBy(s => s.WorkingHours.StartTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting schedules for doctor {DoctorId}", doctorId);
                throw;
            }
        }

        public async Task<IEnumerable<DoctorSchedule>> GetSchedulesByDoctorAndDayAsync(int doctorId, DayOfWeek day)
        {
            try
            {
                return await _dbSet
                    .Include(s => s.Doctor)
                    .Where(s => s.DoctorId == doctorId && s.DayOfWeek == day && !s.IsDeleted)
                    .OrderBy(s => s.WorkingHours.StartTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting schedules for doctor {DoctorId} on day {Day}", doctorId, day);
                throw;
            }
        }

        public async Task<IEnumerable<DoctorSchedule>> GetActiveSchedulesAsync(int doctorId)
        {
            try
            {
                return await _dbSet
                    .Include(s => s.Doctor)
                    .Where(s => s.DoctorId == doctorId && s.Status == ScheduleStatus.Active && !s.IsDeleted)
                    .OrderBy(s => s.DayOfWeek)
                    .ThenBy(s => s.WorkingHours.StartTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active schedules for doctor {DoctorId}", doctorId);
                throw;
            }
        }

        public async Task<DoctorSchedule?> GetScheduleByDoctorAndDayAsync(int doctorId, DayOfWeek day)
        {
            try
            {
                return await _dbSet
                    .Include(s => s.Doctor)
                    .FirstOrDefaultAsync(s => s.DoctorId == doctorId && s.DayOfWeek == day && !s.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting schedule for doctor {DoctorId} on day {Day}", doctorId, day);
                throw;
            }
        }

        public async Task<IEnumerable<DoctorSchedule>> GetSchedulesByDateRangeAsync(int doctorId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var schedules = new List<DoctorSchedule>();
                var currentDate = startDate;

                while (currentDate <= endDate)
                {
                    var daySchedules = await GetSchedulesByDoctorAndDayAsync(doctorId, currentDate.DayOfWeek);
                    
                    var validSchedules = daySchedules.Where(s =>
                        s.ValidFrom <= currentDate &&
                        (!s.ValidTo.HasValue || s.ValidTo >= currentDate) &&
                        s.IsActive);

                    schedules.AddRange(validSchedules);
                    currentDate = currentDate.AddDays(1);
                }

                return schedules;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting schedules for doctor {DoctorId} in date range", doctorId);
                throw;
            }
        }

        public Task<IEnumerable<DoctorSchedule>> GetActiveSchedulesForDateAsync(DateTime date)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DoctorSchedule>> GetSchedulesByTimeSlotAsync(TimeSlot timeSlot)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DoctorSchedule>> GetSchedulesByWorkingHoursAsync(WorkingHours workingHours)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> HasScheduleConflictAsync(int doctorId, DayOfWeek day, TimeSpan startTime, TimeSpan endTime)
        {
            try
            {
                var schedules = await GetSchedulesByDoctorAndDayAsync(doctorId, day);
                
                return schedules.Any(s =>
                    s.IsActive &&
                    s.WorkingHours.StartTime < endTime &&
                    s.WorkingHours.EndTime > startTime);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking schedule conflict for doctor {DoctorId}", doctorId);
                throw;
            }
        }

        public Task<bool> HasScheduleConflictAsync(DoctorSchedule schedule)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DoctorSchedule>> GetConflictingSchedulesAsync(int doctorId, DayOfWeek day, TimeSpan startTime, TimeSpan endTime)
        {
            throw new NotImplementedException();
        }

        public Task AddSchedulesBulkAsync(IEnumerable<DoctorSchedule> schedules)
        {
            throw new NotImplementedException();
        }

        public Task ActivateSchedulesBulkAsync(IEnumerable<int> scheduleIds)
        {
            throw new NotImplementedException();
        }

        public Task DeactivateSchedulesBulkAsync(IEnumerable<int> scheduleIds)
        {
            throw new NotImplementedException();
        }

        public override async Task<DoctorSchedule?> GetByIdAsync(int id)
        {
            try
            {
                return await _dbSet
                    .Include(s => s.Doctor)
                    .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting schedule by id: {Id}", id);
                throw;
            }
        }
    }
}