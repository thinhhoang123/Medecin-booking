using Doctor.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Doctor.Infrastructure.Repositories
{
    public class DoctorRepository : BaseRepository<Doctor>, IDoctorRepository
    {
        private readonly DoctorDbContext _context;

        public DoctorRepository(DoctorDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Doctor?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .Include(d => d.Schedules)
                .FirstOrDefaultAsync(d => d.ContactInfo.Email == email && !d.IsDeleted);
        }

        public async Task<Doctor?> GetByLicenseNumberAsync(string licenseNumber)
        {
            return await _dbSet
                .Include(d => d.Schedules)
                .FirstOrDefaultAsync(d => d.LicenseNumber == licenseNumber && !d.IsDeleted);
        }

        public async Task<IEnumerable<Doctor>> GetBySpecializationAsync(Specialization specialization)
        {
            return await _dbSet
                .Include(d => d.Schedules)
                .Where(d => d.Specialization == specialization && !d.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Doctor>> GetAvailableDoctorsAsync(DateTime date, Specialization? specialization = null)
        {
            var query = _dbSet
                .Include(d => d.Schedules)
                .Where(d => d.Status == DoctorStatus.Active &&
                            d.IsAvailableForAppointments &&
                            !d.IsDeleted);

            if (specialization.HasValue)
            {
                query = query.Where(d => d.Specialization == specialization.Value);
            }

            // Filter doctors that have schedules for the specific day
            var dayOfWeek = date.DayOfWeek;
            query = query.Where(d => d.Schedules.Any(s =>
                s.DayOfWeek == dayOfWeek &&
                s.IsActive &&
                s.ValidFrom <= date &&
                (!s.ValidTo.HasValue || s.ValidTo >= date)));

            return await query.ToListAsync();
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _dbSet.AnyAsync(d => d.ContactInfo.Email == email && !d.IsDeleted);
        }

        public async Task<bool> ExistsByLicenseNumberAsync(string licenseNumber)
        {
            return await _dbSet.AnyAsync(d => d.LicenseNumber == licenseNumber && !d.IsDeleted);
        }

        public async Task<IEnumerable<Doctor>> SearchDoctorsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            return await _dbSet
                .Include(d => d.Schedules)
                .Where(d => (d.FirstName.Contains(searchTerm) ||
                            d.LastName.Contains(searchTerm) ||
                            (d.FirstName + " " + d.LastName).Contains(searchTerm) ||
                            d.ContactInfo.Email.Contains(searchTerm) ||
                            d.Specialization.ToString().Contains(searchTerm) ||
                            (d.Department != null && d.Department.Contains(searchTerm))) &&
                            !d.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Doctor>> GetDoctorsWithSchedulesAsync()
        {
            return await _dbSet
                .Include(d => d.Schedules)
                .Where(d => !d.IsDeleted)
                .ToListAsync();
        }

        public override async Task<Doctor?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(d => d.Schedules)
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
        }
    }
}
