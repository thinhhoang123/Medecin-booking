using AutoMapper;
using Doctor.Application.DTOs;
using Doctor.Application.Interfaces;
using Doctor.Domain.Enums;
using Doctor.Domain.Interfaces;
using Doctor.Domain.ValueObjects;
using DoctorService.Application.DTOs;
using DoctorService.Application.Interfaces;
using DoctorService.Domain.Entities;
using DoctorService.Domain.Enums;
using DoctorService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace DoctorService.Application.Services
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
            var existingDoctor = await _doctorRepository.GetByEmailAsync(doctorDto.Email);
            if (existingDoctor != null)
                throw new InvalidOperationException($"Doctor with email {doctorDto.Email} already exists");

            // Validate license number
            if (!string.IsNullOrEmpty(doctorDto.LicenseNumber))
            {
                var existingLicense = await _doctorRepository.GetByLicenseNumberAsync(doctorDto.LicenseNumber);
                if (existingLicense != null)
                    throw new InvalidOperationException($"Doctor with license number {doctorDto.LicenseNumber} already exists");
            }

            // Parse specialization
            if (!Enum.TryParse<Specialization>(doctorDto.Specialization, true, out var specialization))
                throw new InvalidOperationException($"Invalid specialization: {doctorDto.Specialization}");

            // Create domain entity
            var contactInfo = new ContactInfo(
                doctorDto.Email,
                doctorDto.PhoneNumber,
                doctorDto.MobileNumber,
                doctorDto.Address);

            var doctor = new Doctor(
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
                    var schedule = CreateScheduleFromDto(doctor.Id, scheduleDto);
                    doctor.AddSchedule(schedule);
                }
            }

            // Save
            await _doctorRepository.AddAsync(doctor);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Doctor created: {doctor.Id} - {doctor.Email}");

            // Clear domain events
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

            _logger.LogInformation($"Doctor updated: {doctor.Id} - {doctor.Email}");

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

            _logger.LogInformation($"Doctor deactivated: {doctor.Id} - {doctor.Email}");

            doctor.ClearDomainEvents();

            return true;
        }

        public async Task<bool> DoctorExistsAsync(int id)
        {
            var doctor = await _doctorRepository.GetByIdAsync(id);
            return doctor != null && doctor.Status == DoctorStatus.Active;
        }

        public async Task<IEnumerable<DoctorDto>> SearchDoctorsAsync(DoctorSearchDto searchDto)
        {
            var doctors = await _doctorRepository.SearchDoctorsAsync(searchDto.SearchTerm ?? string.Empty);

            // Filter by specialization
            if (!string.IsNullOrEmpty(searchDto.Specialization))
            {
                if (Enum.TryParse<Specialization>(searchDto.Specialization, true, out var spec))
                {
                    doctors = doctors.Where(d => d.Specialization == spec);
                }
            }

            // Filter by availability
            if (searchDto.AvailableDate.HasValue)
            {
                var availableDoctors = await _doctorRepository.GetAvailableDoctorsAsync(
                    searchDto.AvailableDate.Value);
                doctors = doctors.Where(d => availableDoctors.Any(ad => ad.Id == d.Id));
            }

            // Apply pagination
            var pagedDoctors = doctors
                .Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize);

            return _mapper.Map<IEnumerable<DoctorDto>>(pagedDoctors);
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
           