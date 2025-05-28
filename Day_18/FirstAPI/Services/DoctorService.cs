using FirstAPI.Models;
using FirstAPI.Interfaces;
using FirstAPI.Models.DTOs;
using FirstAPI.Exceptions;

namespace FirstAPI.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IRepository<int, Doctor> _doctorRepository;
        private readonly IRepository<int, Speciality> _specialityRepository;
        private readonly IRepository<int, DoctorSpeciality> _doctorSpecialityRepository;

        public DoctorService(IRepository<int, Doctor> doctorRepository,
                             IRepository<int, Speciality> specialityRepository,
                             IRepository<int, DoctorSpeciality> doctorSpecialityRepository)
        {
            _doctorRepository = doctorRepository;
            _specialityRepository = specialityRepository;
            _doctorSpecialityRepository = doctorSpecialityRepository;
        }

        public async Task<Doctor> AddDoctor(DoctorAddRequestDto doctorDto)
        {
            var doctor = new Doctor
            {
                Name = doctorDto.Name,
                YearsOfExperience = doctorDto.YearsOfExperience,
                Status = "Active",
                DoctorSpecialities = new List<DoctorSpeciality>()
            };

            if (doctorDto.Specialities == null)
            {
                throw new Exception("Please specify Specialities");
            }

            var allSpecialities = await _specialityRepository.GetAll();

            foreach (var specDto in doctorDto.Specialities)
            {
                var speciality = allSpecialities.FirstOrDefault(s => s.Name.Equals(specDto.Name, StringComparison.OrdinalIgnoreCase));

                if (speciality == null)
                {
                    speciality = await _specialityRepository.Add(new Speciality { Name = specDto.Name, Status = "Active" });
                }

                doctor.DoctorSpecialities.Add(new DoctorSpeciality
                {
                    SpecialityId = speciality.Id,
                    Doctor = doctor
                });
            }


            return await _doctorRepository.Add(doctor);
        }



        public async Task<Doctor> GetDoctorByName(string name)
        {
            var doctors = await _doctorRepository.GetAll();
            var doctor = doctors.FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (doctor == null)
                throw new NotFoundException($"Doctor with name '{name}' not found.");

            return doctor;
        }

        public async Task<ICollection<Doctor>> GetDoctorBySpeciality(string speciality)
        {
            var specialities = await _specialityRepository.GetAll();
            var specialityFound = specialities.FirstOrDefault(sp => sp.Name.Equals(speciality, StringComparison.OrdinalIgnoreCase));

            if (specialityFound == null)
            {
                throw new NotFoundException("Speciality not found");
            }

            var allDoctorSpecialities = await _doctorSpecialityRepository.GetAll();
            var specialityDoctorsId = allDoctorSpecialities.Where(ds => ds.SpecialityId == specialityFound.Id)
                                                           .Select(ds => ds.DoctorId)
                                                           .ToList();

            var doctors = await _doctorRepository.GetAll();
            var doctorsWithSpeciality = doctors.Where(d => specialityDoctorsId.Contains(d.Id)).ToList();

            if (doctorsWithSpeciality == null)
            {
                throw new NotFoundException($"No doctors found with the speciality: {speciality}");
            }

            return doctorsWithSpeciality;
        }
        
        public async Task<Doctor> UpdateDoctor(int id, DoctorAddRequestDto doctorDto)
        {
            var existingDoctor = await _doctorRepository.Get(id);
            if (existingDoctor == null)
                throw new Exception($"Doctor with ID {id} not found.");

            existingDoctor.Name = doctorDto.Name;
            existingDoctor.YearsOfExperience = doctorDto.YearsOfExperience;

            if (doctorDto.Specialities != null && doctorDto.Specialities.Any())
            {
                var allSpecialities = await _specialityRepository.GetAll();
                var updatedSpecialities = new List<DoctorSpeciality>();

                foreach (var specialityDto in doctorDto.Specialities)
                {
                    var speciality = allSpecialities.FirstOrDefault(s =>
                        s.Name.Equals(specialityDto.Name, StringComparison.OrdinalIgnoreCase));

                    if (speciality == null)
                        throw new Exception($"Speciality '{specialityDto.Name}' not found.");

                    updatedSpecialities.Add(new DoctorSpeciality
                    {
                        DoctorId = id,
                        SpecialityId = speciality.Id
                    });
                }

                existingDoctor.DoctorSpecialities = updatedSpecialities;
            }

            var updatedDoctor = await _doctorRepository.Update(id, existingDoctor);
            return updatedDoctor;
        }


        public async Task<Doctor> DeleteDoctorById(int id)
        {
            var doctor = await _doctorRepository.Get(id);

            if (doctor == null)
                throw new Exception($"Doctor with ID {id} not found.");

            return await _doctorRepository.Delete(id);
        }


    }
}
