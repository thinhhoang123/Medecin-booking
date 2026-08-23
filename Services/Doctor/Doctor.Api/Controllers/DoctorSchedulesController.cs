using Doctor.Application.DTOs;
using Doctor.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Doctor.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class DoctorSchedulesController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly ILogger<DoctorSchedulesController> _logger;

        public DoctorSchedulesController(IDoctorService doctorService, ILogger<DoctorSchedulesController> logger)
        {
            _doctorService = doctorService;
            _logger = logger;
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<ActionResult<IEnumerable<DoctorScheduleDto>>> GetSchedulesByDoctor(int doctorId)
        {
            try
            {
                var schedules = await _doctorService.GetDoctorSchedulesAsync(doctorId);
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting schedules for doctor {DoctorId}", doctorId);
                return StatusCode(500, "An error occurred while retrieving schedules");
            }
        }

        [HttpGet("doctor/{doctorId}/active")]
        public async Task<ActionResult<IEnumerable<DoctorScheduleDto>>> GetActiveSchedules(int doctorId)
        {
            try
            {
                var schedules = await _doctorService.GetActiveSchedulesAsync(doctorId);
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active schedules for doctor {DoctorId}", doctorId);
                return StatusCode(500, "An error occurred while retrieving active schedules");
            }
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<DoctorScheduleDto>> AddSchedule([FromBody] CreateDoctorScheduleDto scheduleDto)
        {
            try
            {
                var schedule = await _doctorService.AddScheduleAsync(scheduleDto);
                return CreatedAtAction(nameof(GetSchedulesByDoctor), new { doctorId = schedule.DoctorId }, schedule);
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
                _logger.LogError(ex, "Error adding schedule");
                return StatusCode(500, "An error occurred while adding the schedule");
            }
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<DoctorScheduleDto>> UpdateSchedule(int id, [FromBody] UpdateDoctorScheduleDto scheduleDto)
        {
            try
            {
                var schedule = await _doctorService.UpdateScheduleAsync(id, scheduleDto);
                return Ok(schedule);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating schedule {Id}", id);
                return StatusCode(500, "An error occurred while updating the schedule");
            }
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> RemoveSchedule(int id)
        {
            try
            {
                var result = await _doctorService.RemoveScheduleAsync(id);
                if (!result)
                    return NotFound($"Schedule with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing schedule {Id}", id);
                return StatusCode(500, "An error occurred while removing the schedule");
            }
        }
    }
}