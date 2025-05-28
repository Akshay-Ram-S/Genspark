using FirstAPI.Models;
using FirstAPI.Models.DTOs;

namespace FirstAPI.Interfaces
{
    public interface IDoctorService
    {
        public Task<Doctor> GetDoctorByName(string name);
        public Task<ICollection<Doctor>> GetDoctorBySpeciality(string speciality);
        public Task<Doctor> AddDoctor(DoctorAddRequestDto doctor);
        public Task<Doctor> UpdateDoctor(int id, DoctorAddRequestDto doctoDto);
        public Task<Doctor> DeleteDoctorById(int id);

    }
}