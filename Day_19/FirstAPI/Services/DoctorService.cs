using System.Threading.Tasks;
using FirstAPI.Interfaces;
using FirstAPI.Misc;
using FirstAPI.Models;
using FirstAPI.Models.DTOs;
using Microsoft.VisualBasic;

namespace FirstAPI.Services
{
    public class DoctorService : IDoctorService
    {
        DoctorMapper doctorMapper ;
        SpecialityMapper specialityMapper;
        IOtherContextFunctionities _otherContextFunctionities;
        private readonly IRepository<int, Doctor> _doctorRepository;
        private readonly IRepository<int, Speciality> _specialityRepository;
        private readonly IRepository<int, DoctorSpeciality> _doctorSpecialityRepository;

        public DoctorService(IRepository<int, Doctor> doctorRepository,
                            IRepository<int, Speciality> specialityRepository,
                            IRepository<int, DoctorSpeciality> doctorSpecialityRepository,
                            IOtherContextFunctionities otherContextFunctionities)
        {
            doctorMapper = new DoctorMapper();
            specialityMapper = new();
            _doctorRepository = doctorRepository;
            _specialityRepository = specialityRepository;
            _doctorSpecialityRepository = doctorSpecialityRepository;
            _otherContextFunctionities = otherContextFunctionities;            

        }

        public async Task<Doctor> AddDoctor(DoctorAddRequestDto doctor)
        {
            try
            {
                var newDoctor = doctorMapper.MapDoctorAddRequestDoctor(doctor);
                newDoctor = await _doctorRepository.Add(newDoctor);
                if (newDoctor == null)
                    throw new Exception("Could not add doctor");
                if (doctor.Specialities.Count() > 0)
                {
                    int[] specialities = await MapAndAddSpeciality(doctor);
                    for (int i = 0; i < specialities.Length; i++)
                    {
                        var doctorSpeciality = specialityMapper.MapDoctorSpecility(newDoctor.Id, specialities[i]);
                        doctorSpeciality = await _doctorSpecialityRepository.Add(doctorSpeciality);
                    }
                }
                return newDoctor;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            
        }

        private async Task<int[]> MapAndAddSpeciality(DoctorAddRequestDto doctor)
        {
            int[] specialityIds = new int[doctor.Specialities.Count()];
            IEnumerable<Speciality> existingSpecialities = null;
            try
            {
                existingSpecialities = await _specialityRepository.GetAll();
            }
            catch (Exception e)
            {

            }
            int count = 0;
            foreach (var item in doctor.Specialities)
            {
                Speciality? speciality = null;
                if (existingSpecialities != null)
                    speciality = existingSpecialities.FirstOrDefault(s => s.Name.ToLower() == item.Name.ToLower());
                if (speciality == null)
                {
                    speciality = specialityMapper.MapSpecialityAddRequestDoctor(item);
                    speciality = await _specialityRepository.Add(speciality);
                }
                specialityIds[count] = speciality.Id;
                count++;
            }
            return specialityIds;
        }

        public Task<Doctor> GetDoctorByName(string name)
        {
            throw new NotImplementedException();
        }


        public Task<ICollection<DoctorsBySpecialityResponseDto>> GetDoctorsBySpeciality(string speciality)
        {
            throw new NotImplementedException();
        }
    }
}

