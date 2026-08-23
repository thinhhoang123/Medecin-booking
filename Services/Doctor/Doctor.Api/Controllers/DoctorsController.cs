using Doctor.Application.DTOs;
using Doctor.Application.Interfaces;
using Doctor.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Doctor.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly ILogger<DoctorsController> _logger;

        public DoctorsController(IDoctorService doctorService, ILogger<DoctorsController> logger)
        {
            _doctorService = doctorService;
            _logger = logger;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorDto>>> GetAllDoctors()
        {
            try
            {
                var doctors = await _doctorService.GetAllDoctorsAsync();
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all doctors");
                return StatusCode(500, "An error occurred while retrieving doctors");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorDto>> GetDoctorById(int id)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByIdAsync(id);
                if (doctor == null)
                    return NotFound($"Doctor with ID {id} not found");

                return Ok(doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctor by id: {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the doctor");
            }
        }

        [HttpGet("by-email")]
        public async Task<ActionResult<DoctorDto>> GetDoctorByEmail([FromQuery] string email)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByEmailAsync(email);
                if (doctor == null)
                    return NotFound($"Doctor with email {email} not found");

                return Ok(doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctor by email: {Email}", email);
                return StatusCode(500, "An error occurred while retrieving the doctor");
            }
        }

        [HttpGet("by-license")]
        public async Task<ActionResult<DoctorDto>> GetDoctorByLicense([FromQuery] string licenseNumber)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByLicenseNumberAsync(licenseNumber);
                if (doctor == null)
                    return NotFound($"Doctor with license number {licenseNumber} not found");

                return Ok(doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctor by license: {LicenseNumber}", licenseNumber);
                return StatusCode(500, "An error occurred while retrieving the doctor");
            }
        }

        [HttpGet("specialization/{specialization}")]
        public async Task<ActionResult<IEnumerable<DoctorDto>>> GetDoctorsBySpecialization(string specialization)
        {
            try
            {
                var doctors = await _doctorService.GetDoctorsBySpecializationAsync(specialization);
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctors by specialization: {Specialization}", specialization);
                return StatusCode(500, "An error occurred while retrieving doctors");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<DoctorDto>>> SearchDoctors([FromQuery] string searchTerm)
        {
            try
            {
                var doctors = await _doctorService.SearchDoctorsAsync(searchTerm);
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching doctors with term: {SearchTerm}", searchTerm);
                return StatusCode(500, "An error occurred while searching doctors");
            }
        }

        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<DoctorDto>>> GetAvailableDoctors(
            [FromQuery] DateTime date,
            [FromQuery] string? specialization = null)
        {
            try
            {
                var doctors = await _doctorService.GetAvailableDoctorsAsync(date, specialization);
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available doctors");
                return StatusCode(500, "An error occurred while retrieving available doctors");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DoctorDto>> CreateDoctor([FromBody] CreateDoctorDto createDto)
        {
            try
            {
                var doctor = await _doctorService.CreateDoctorAsync(createDto);
                return CreatedAtAction(nameof(GetDoctorById), new { id = doctor.Id }, doctor);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating doctor");
                return StatusCode(500, "An error occurred while creating the doctor");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DoctorDto>> UpdateDoctor(int id, [FromBody] UpdateDoctorDto updateDto)
        {
            try
            {
                var doctor = await _doctorService.UpdateDoctorAsync(id, updateDto);
                return Ok(doctor);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating doctor with id: {Id}", id);
                return StatusCode(500, "An error occurred while updating the doctor");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteDoctor(int id)
        {
            try
            {
                var result = await _doctorService.DeleteDoctorAsync(id);
                if (!result)
                    return NotFound($"Doctor with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting doctor with id: {Id}", id);
                return StatusCode(500, "An error occurred while deleting the doctor");
            }
        }

        [HttpGet("{id}/schedules")]
        public async Task<ActionResult<IEnumerable<DoctorScheduleDto>>> GetDoctorSchedules(int id)
        {
            try
            {
                var schedules = await _doctorService.GetDoctorSchedulesAsync(id);
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting schedules for doctor: {Id}", id);
                return StatusCode(500, "An error occurred while retrieving schedules");
            }
        }

        [HttpGet("{id}/availability")]
        public async Task<ActionResult<IEnumerable<TimeSlot>>> GetAvailableTimeSlots(
            int id,
            [FromQuery] DateTime date)
        {
            try
            {
                var slots = await _doctorService.GetAvailableTimeSlotsAsync(id, date);
                return Ok(slots);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available time slots for doctor: {Id}", id);
                return StatusCode(500, "An error occurred while retrieving available time slots");
            }
        }

        [HttpGet("check-availability")]
        public async Task<ActionResult<bool>> CheckAvailability(
            [FromQuery] int doctorId,
            [FromQuery] DateTime date,
            [FromQuery] string startTime,
            [FromQuery] string endTime)
        {
            try
            {
                if (!TimeSpan.TryParse(startTime, out var start))
                    return BadRequest("Invalid start time format");

                if (!TimeSpan.TryParse(endTime, out var end))
                    return BadRequest("Invalid end time format");

                var isAvailable = await _doctorService.CheckAvailabilityAsync(doctorId, date, start, end);
                return Ok(isAvailable);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking availability");
                return StatusCode(500, "An error occurred while checking availability");
            }
        }
    }
}