using Microsoft.AspNetCore.Mvc;
using FirstAPI.Models;

namespace FirstAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class DoctorController : ControllerBase
    {
        static List<Doctor> doctors = new List<Doctor>
        {
            new Doctor{Id=101,Name="Ramu"},
            new Doctor{Id=102,Name="Somu"},
        };
        [HttpGet]
        public ActionResult<IEnumerable<Doctor>> GetDoctors()
        {
            return Ok(doctors);
        }

        [HttpPost]
        public ActionResult<Doctor> PostDoctor([FromBody] Doctor doctor)
        {
            doctors.Add(doctor);
            return Created("", doctor);
        }

        [HttpPut]
        public ActionResult<Doctor> UpdateDoctor([FromBody] Doctor doctor)
        {
            var doctorRecord = doctors.FirstOrDefault(d => d.Id == doctor.Id);
            if (doctorRecord == null)
            {
                return NotFound($"Doctor Id: {doctor.Id} not found.");
            }

            doctorRecord.Name = doctor.Name;
            return Ok(doctorRecord);
        }

        [HttpDelete("{id}")]
        public ActionResult<Doctor> DeleteDoctor(int id)
        {
            var doctorRecord = doctors.FirstOrDefault(d => d.Id == id);
            if (doctorRecord == null)
            {
                return NotFound($"Doctor Id: {id} not found.");
            }

            doctors.Remove(doctorRecord);
            return NoContent();
        }
    }
}