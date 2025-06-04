
using Microsoft.AspNetCore.Mvc;
using FirstAPI.Interfaces;
using FirstAPI.Models;
using FirstAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace FirstAPI.Controllers
{


    [ApiController]
    [Route("/api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpPost]
        public async Task<ActionResult<Appointmnet>> PostPatient([FromBody] AppointmentFixDto appointment)
        {
            try
            {
                var newAppointment = await _appointmentService.BookAppointment(appointment);
                if (newAppointment != null)
                    return Created("", newAppointment);
                return BadRequest("Unable to process request at this moment");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("cancel")]
        [Authorize(Policy = "DoctorsWithExperience")]
        public async Task<ActionResult<Appointmnet>> CancelAppointmentByDoctor([FromBody] string key)
        {
            try
            {
                var cancelAppointment = await _appointmentService.CancelAppointment(key);
                if (cancelAppointment != null)
                {
                    return Ok(cancelAppointment);
                }
                return BadRequest("Unable to process at this moment");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}