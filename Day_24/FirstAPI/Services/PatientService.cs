using AutoMapper;
using FirstAPI.Interfaces;
using FirstAPI.Misc;
using FirstAPI.Models;
using FirstAPI.Models.DTOs;

namespace FirstAPI.Services
{
    public class PatientService : IPatientService
    {
        private readonly IMapper _mapper;
        PatientMapper _patientMapper;
        private IRepository<int, Patient> _patientRepository;
        private readonly IRepository<string, User> _userRepository;
        private IOtherContextFunctionities _otherContextFunctionities;
        private readonly IEncryptionService _encryptionService;

        public PatientService(IRepository<int, Patient> patientRepository,
                            IRepository<string,User> userRepository,
                            IOtherContextFunctionities otherContextFunctionities,
                            IEncryptionService encryptionService,
                            IMapper mapper)
        {
            _patientMapper = new PatientMapper();
            _patientRepository = patientRepository;
            _userRepository = userRepository;
            _otherContextFunctionities = otherContextFunctionities;
            _encryptionService = encryptionService;
            _mapper = mapper;

        }
        
        public async Task<Patient> AddPatient(PatientAddRequestDto patient)
        {

            try
            {
                var user = _mapper.Map<PatientAddRequestDto, User>(patient);
                var encryptedData = await _encryptionService.EncryptData(new EncryptModel
                {
                    Data = patient.Password
                });
                user.Password = encryptedData.EncryptedData;
                user.Role = "Patient";
                user = await _userRepository.Add(user);
                var newPatient = _patientMapper.MapPatientAddRequest(patient);
                newPatient = await _patientRepository.Add(newPatient);
                return newPatient;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }

}