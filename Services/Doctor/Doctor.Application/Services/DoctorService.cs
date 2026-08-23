using AutoMapper;
using Doctor.Application.DTOs;
using Doctor.Application.Interfaces;
using Doctor.Domain.Entities;
using Doctor.Domain.Enums;
using Doctor.Domain.Interfaces;
using Doctor.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Doctor.Application.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IDoctorScheduleRepository _scheduleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<DoctorService> _logger;

        public DoctorService(
            IDoctorRepository doctorRepository,
            IDoctorScheduleRepository scheduleRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<DoctorService> logger)
        {
            _doctorRepository = doctorRepository;
            _scheduleRepository = scheduleRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync()
        {
            var doctors = await _doctorRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }

        public async Task<DoctorDto?> GetDoctorByIdAsync(int id)
        {
            var doctor = await _doctorRepository.GetByIdAsync(id);
            return doctor == null ? null : _mapper.Map<DoctorDto>(doctor);
        }

        public async Task<DoctorDto?> GetDoctorByEmailAsync(string email)
        {
            var doctor = await _doctorRepository.GetByEmailAsync(email);
            return doctor == null ? null : _mapper.Map<DoctorDto>(doctor);
        }

        public async Task<DoctorDto?> GetDoctorByLicenseNumberAsync(string licenseNumber)
        {
            var doctor = await _doctorRepository.GetByLicenseNumberAsync(licenseNumber);
            return doctor == null ? null : _mapper.Map<DoctorDto>(doctor);
        }

        public async Task<IEnumerable<DoctorDto>> GetDoctorsBySpecializationAsync(string specialization)
        {
            if (!Enum.TryParse<Specialization>(specialization, true, out var spec))
                return Enumerable.Empty<DoctorDto>();

            var doctors = await _doctorRepository.GetBySpecializationAsync(spec);
            return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }

        public async Task<DoctorDto> CreateDoctorAsync(CreateDoctorDto doctorDto)
        {
            // Validate email
            if (await _doctorRepository.ExistsByEmailAsync(doctorDto.Email))
                throw new InvalidOperationException($"Doctor with email {doctorDto.Email} already exists");

            // Parse specialization
            if (!Enum.TryParse<Specialization>(doctorDto.Specialization, true, out var specialization))
                throw new InvalidOperationException($"Invalid specialization: {doctorDto.Specialization}");

            // Create contact info
            var contactInfo = new ContactInfo(
                doctorDto.Email,
                doctorDto.PhoneNumber,
                doctorDto.MobileNumber,
                doctorDto.Address);

            // Create doctor
            var doctor = new Domain.Entities.Doctor(
                doctorDto.FirstName,
                doctorDto.LastName,
                specialization,
                contactInfo,
                doctorDto.Bio,
                doctorDto.Qualifications,
                doctorDto.LicenseNumber,
                doctorDto.Department,
                doctorDto.UserId);

            // Add schedules if provided
            if (doctorDto.Schedules != null && doctorDto.Schedules.Any())
            {
                foreach (var scheduleDto in doctorDto.Schedules)
                {
                    if (!Enum.TryParse<DayOfWeek>(scheduleDto.DayOfWeek, true, out var dayOfWeek))
                        throw new InvalidOperationException($"Invalid day of week: {scheduleDto.DayOfWeek}");

                    var workingHours = new WorkingHours(scheduleDto.StartTime, scheduleDto.EndTime);
                    var schedule = new DoctorSchedule(
                        doctor.Id,
                        dayOfWeek,
                        workingHours,
                        scheduleDto.SlotDurationInMinutes,
                        scheduleDto.ValidFrom,
                        scheduleDto.ValidTo);

                    doctor.AddSchedule(schedule);
                }
            }

            await _doctorRepository.AddAsync(doctor);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Doctor created: {doctor.Id}");
            doctor.ClearDomainEvents();

            return _mapper.Map<DoctorDto>(doctor);
        }

        public async Task<DoctorDto> UpdateDoctorAsync(int id, UpdateDoctorDto doctorDto)
        {
            var doctor = await _doctorRepository.GetByIdAsync(id);
            if (doctor == null)
                throw new KeyNotFoundException($"Doctor with ID {id} not found");

            // Update basic info
            var firstName = doctorDto.FirstName ?? doctor.FirstName;
            var lastName = doctorDto.LastName ?? doctor.LastName;
            var specialization = doctor.Specialization;

            if (!string.IsNullOrEmpty(doctorDto.Specialization))
            {
                if (!Enum.TryParse<Specialization>(doctorDto.Specialization, true, out var spec))
                    throw new InvalidOperationException($"Invalid specialization: {doctorDto.Specialization}");
                specialization = spec;
            }

            var contactInfo = new ContactInfo(
                doctorDto.Email ?? doctor.ContactInfo.Email,
                doctorDto.PhoneNumber ?? doctor.ContactInfo.PhoneNumber,
                doctorDto.MobileNumber ?? doctor.ContactInfo.MobileNumber,
                doctorDto.Address ?? doctor.ContactInfo.Address);

            doctor.UpdateProfile(
                firstName,
                lastName,
                specialization,
                contactInfo,
                doctorDto.Bio ?? doctor.Bio,
                doctorDto.Qualifications ?? doctor.Qualifications,
                doctorDto.Department ?? doctor.Department);

            // Update availability
            if (doctorDto.IsAvailableForAppointments.HasValue)
            {
                doctor.UpdateAvailability(doctorDto.IsAvailableForAppointments.Value);
            }

            // Update status
            if (!string.IsNullOrEmpty(doctorDto.Status))
            {
                if (Enum.TryParse<DoctorStatus>(doctorDto.Status, true, out var status))
                {
                    doctor.UpdateStatus(status);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Doctor updated: {doctor.Id}");
            doctor.ClearDomainEvents();

            return _mapper.Map<DoctorDto>(doctor);
        }

        public async Task<bool> DeleteDoctorAsync(int id)
        {
            var doctor = await _doctorRepository.GetByIdAsync(id);
            if (doctor == null)
                return false;

            doctor.UpdateStatus(DoctorStatus.Inactive);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Doctor deactivated: {doctor.Id}");
            doctor.ClearDomainEvents();

            return true;
        }

        public async Task<bool> DoctorExistsAsync(int id)
        {
            return await _doctorRepository.ExistsAsync(d => d.Id == id && d.Status == DoctorStatus.Active);
        }

        public async Task<IEnumerable<DoctorDto>> SearchDoctorsAsync(string searchTerm)
        {
            var doctors = await _doctorRepository.SearchDoctorsAsync(searchTerm);
            return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }

        public async Task<IEnumerable<DoctorDto>> GetAvailableDoctorsAsync(DateTime date, string? specialization = null)
        {
            var specializationEnum = string.IsNullOrEmpty(specialization) 
                ? (Specialization?)null 
                : Enum.Parse<Specialization>(specialization, true);

            var doctors = await _doctorRepository.GetAvailableDoctorsAsync(date, specializationEnum);
            return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }

        public async Task<IEnumerable<DoctorScheduleDto>> GetDoctorSchedulesAsync(int doctorId)
        {
            var schedules = await _scheduleRepository.GetSchedulesByDoctorAsync(doctorId);
            return _mapper.Map<IEnumerable<DoctorScheduleDto>>(schedules);
        }

        public async Task<DoctorScheduleDto> AddScheduleAsync(CreateDoctorScheduleDto scheduleDto)
        {
            if (!Enum.TryParse<DayOfWeek>(scheduleDto.DayOfWeek, true, out var dayOfWeek))
                throw new InvalidOperationException($"Invalid day of week: {scheduleDto.DayOfWeek}");

            // Check if doctor exists
            if (!await DoctorExistsAsync(scheduleDto.DoctorId))
                throw new KeyNotFoundException($"Doctor with ID {scheduleDto.DoctorId} not found");

            // Check for conflicts
            var hasConflict = await _scheduleRepository.HasScheduleConflictAsync(
                scheduleDto.DoctorId,
                dayOfWeek,
                scheduleDto.StartTime,
                scheduleDto.EndTime);

            if (hasConflict)
                throw new InvalidOperationException("Schedule conflict detected");

            var workingHours = new WorkingHours(scheduleDto.StartTime, scheduleDto.EndTime);
            var schedule = new DoctorSchedule(
                scheduleDto.DoctorId,
                dayOfWeek,
                workingHours,
                scheduleDto.SlotDurationInMinutes,
                scheduleDto.ValidFrom,
                scheduleDto.ValidTo);

            var doctor = await _doctorRepository.GetByIdAsync(scheduleDto.DoctorId);
            doctor.AddSchedule(schedule);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Schedule added for doctor {scheduleDto.DoctorId}");
            return _mapper.Map<DoctorScheduleDto>(schedule);
        }

        public async Task<DoctorScheduleDto> UpdateScheduleAsync(int scheduleId, UpdateDoctorScheduleDto scheduleDto)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);
            if (schedule == null)
                throw new KeyNotFoundException($"Schedule with ID {scheduleId} not found");

            if (scheduleDto.StartTime.HasValue && scheduleDto.EndTime.HasValue)
            {
                var workingHours = new WorkingHours(scheduleDto.StartTime.Value, scheduleDto.EndTime.Value);
                schedule.UpdateWorkingHours(workingHours);
            }

            if (scheduleDto.SlotDurationInMinutes.HasValue)
                schedule.UpdateSlotDuration(scheduleDto.SlotDurationInMinutes.Value);

            if (!string.IsNullOrEmpty(scheduleDto.Status))
            {
                if (Enum.TryParse<ScheduleStatus>(scheduleDto.Status, true, out var status))
                {
                    if (status == ScheduleStatus.Active)
                        schedule.Activate();
                    else
                        schedule.Deactivate();
                }
            }

            if (scheduleDto.ValidFrom.HasValue || scheduleDto.ValidTo.HasValue)
                schedule.SetValidityPeriod(scheduleDto.ValidFrom, scheduleDto.ValidTo);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Schedule updated: {scheduleId}");
            return _mapper.Map<DoctorScheduleDto>(schedule);
        }

        public async Task<bool> RemoveScheduleAsync(int scheduleId)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);
            if (schedule == null)
                return false;

            schedule.Deactivate();
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Schedule deactivated: {scheduleId}");
            return true;
        }

        public async Task<IEnumerable<DoctorScheduleDto>> GetActiveSchedulesAsync(int doctorId)
        {
            var schedules = await _scheduleRepository.GetActiveSchedulesAsync(doctorId);
            return _mapper.Map<IEnumerable<DoctorScheduleDto>>(schedules);
        }

        public async Task<IEnumerable<TimeSlot>> GetAvailableTimeSlotsAsync(int doctorId, DateTime date)
        {
            var doctor = await _doctorRepository.GetByIdAsync(doctorId);
            if (doctor == null)
                throw new KeyNotFoundException($"Doctor with ID {doctorId} not found");

            return doctor.GetAvailableTimeSlots(date);
        }

        public async Task<bool> CheckAvailabilityAsync(int doctorId, DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            var doctor = await _doctorRepository.GetByIdAsync(doctorId);
            if (doctor == null)
                return false;

            return doctor.IsAvailableOn(date, startTime, endTime);
        }
    }
}