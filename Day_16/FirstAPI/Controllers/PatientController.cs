using Microsoft.AspNetCore.Mvc;
using FirstAPI.Models;

namespace FirstAPI.Controllers {
    
    [ApiController]
    [Route("/api/[controller]")]
    public class PatientController : ControllerBase
    {
        static List<Patient> patients = new List<Patient>
        {
            new Patient{PatientId=101, PatientName="Alice", PatientAge=25, Phone="8000011111"},
            new Patient{PatientId=102,PatientName="Bob", PatientAge=23, Phone="9000011111"},
        };
        [HttpGet]
        public ActionResult<IEnumerable<Patient>> GetPatients()
        {
            return Ok(patients);
        }

        [HttpPost]
        public ActionResult<Patient> AddPatient([FromBody] Patient patient)
        {
            if (patients.Any(p => p.PatientId == patient.PatientId))
            {
                return Conflict(new { message = "A patient with the same ID already exists." });
            }

            patients.Add(patient);
            return Created("", patient);
        }

        [HttpPut]
        public ActionResult<Patient> UpdatePatient([FromBody] Patient patient)
        {
            var index = patients.FindIndex(p => p.PatientId == patient.PatientId);
            if (index == -1)
            {
                return NotFound($"Patient ID: {patient.PatientId} is not found.");
            }

            patients[index] = patient;
            return Ok(patient);
        }


        [HttpDelete("{id}")]
        public ActionResult DeletePatient(int id)
        {
            var patientRecord = patients.FirstOrDefault(p => p.PatientId == id);
            if (patientRecord == null)
            {
                return NotFound($"Patient ID: {id} is not found.");
            }
            patients.Remove(patientRecord);
            return NoContent();
        }

    }
}
