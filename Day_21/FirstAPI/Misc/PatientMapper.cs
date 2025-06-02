using FirstAPI.Models;
using FirstAPI.Models.DTOs;

namespace FirstAPI.Misc
{
    public class PatientMapper
    {
        public Patient? MapPatientAddRequest(PatientAddRequestDto addRequestDto)
        {
            Patient patient = new();
            patient.Name = addRequestDto.Name;
            patient.Age = addRequestDto.Age;
            patient.Phone = addRequestDto.Phone;
            patient.Email = addRequestDto.Email;
            return patient;
        }
    }

}