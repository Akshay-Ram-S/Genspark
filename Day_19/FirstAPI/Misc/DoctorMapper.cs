using FirstAPI.Models;
using FirstAPI.Models.DTOs;

namespace FirstAPI.Misc
{
    public class DoctorMapper
    {
        public Doctor? MapDoctorAddRequestDoctor(DoctorAddRequestDto addRequestDto)
        {
            Doctor doctor = new();
            doctor.Name = addRequestDto.Name;
            doctor.YearsOfExperience = addRequestDto.YearsOfExperience;
            doctor.Status = "Active";
            return doctor;
        }
    }
}