using FirstAPI.Interfaces;
using FirstAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FirstAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorsController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByName(name);
                return Ok(doctor);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("speciality/{speciality}")]
        public async Task<IActionResult> GetBySpeciality(string speciality)
        {
            try
            {
                var doctors = await _doctorService.GetDoctorBySpeciality(speciality);
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddDoctor([FromBody] DoctorAddRequestDto doctorDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var doctor = await _doctorService.AddDoctor(doctorDto);
                return Ok(doctor);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctor(int id, [FromBody] DoctorAddRequestDto doctorDto)
        {
            var updatedDoctor = await _doctorService.UpdateDoctor(id, doctorDto);
            return Ok(updatedDoctor);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var deletedDoctor = await _doctorService.DeleteDoctorById(id);
            return Ok(deletedDoctor);
        }
    }
}
