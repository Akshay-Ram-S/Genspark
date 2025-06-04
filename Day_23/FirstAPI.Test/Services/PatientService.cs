using System.Threading.Tasks;
using AutoMapper;
using FirstAPI.Interfaces;
using FirstAPI.Misc;
using FirstAPI.Models;
using FirstAPI.Models.DTOs;
using FirstAPI.Services;
using Moq;
using NUnit.Framework;

namespace FirstAPI.Test
{
    public class PatientServiceUnitTests
    {
        private Mock<IRepository<int, Patient>> _mockPatientRepo;
        private Mock<IRepository<string, User>> _mockUserRepo;
        private Mock<IEncryptionService> _mockEncryptionService;
        private Mock<IMapper> _mockMapper;
        private Mock<IOtherContextFunctionities> _mockContextExtras;
        private PatientService _patientService;

        [SetUp]
        public void Init()
        {
            _mockPatientRepo = new Mock<IRepository<int, Patient>>();
            _mockUserRepo = new Mock<IRepository<string, User>>();
            _mockEncryptionService = new Mock<IEncryptionService>();
            _mockMapper = new Mock<IMapper>();
            _mockContextExtras = new Mock<IOtherContextFunctionities>();

            _patientService = new PatientService(
                _mockPatientRepo.Object,
                _mockUserRepo.Object,
                _mockContextExtras.Object,
                _mockEncryptionService.Object,
                _mockMapper.Object
            );
        }

        [Test]
        public async Task AddPatient_ReturnsExpectedPatient_OnValidInput()
        {
            // Arrange
            var dto = new PatientAddRequestDto
            {
                Name = "Alice",
                Email = "alice@example.com",
                Password = "strong123"
            };

            var encrypted = new EncryptModel
            {
                Data = dto.Password,
                EncryptedData = "encryptedString"
            };

            var user = new User
            {
                Username = dto.Email,
                Password = encrypted.EncryptedData,
                Role = "Patient"
            };

            var patient = new Patient
            {
                Id = 1,
                Name = dto.Name,
                Email = dto.Email
            };

            _mockEncryptionService.Setup(e => e.EncryptData(It.IsAny<EncryptModel>()))
                                  .ReturnsAsync(encrypted);
            _mockMapper.Setup(m => m.Map<PatientAddRequestDto, User>(dto)).Returns(user);
            _mockMapper.Setup(m => m.Map<Patient>(dto)).Returns(patient);
            _mockUserRepo.Setup(r => r.Add(It.IsAny<User>())).ReturnsAsync(user);
            _mockPatientRepo.Setup(r => r.Add(It.IsAny<Patient>())).ReturnsAsync(patient);

            // Act
            var result = await _patientService.AddPatient(dto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Alice"));
            Assert.That(result.Email, Is.EqualTo("alice@example.com"));
        }

        [Test]
        public void AddPatient_ThrowsException_WhenEncryptionFails()
        {
            // Arrange
            var dto = new PatientAddRequestDto
            {
                Name = "Bob",
                Email = "bob@example.com",
                Password = "password123"
            };

            _mockEncryptionService.Setup(e => e.EncryptData(It.IsAny<EncryptModel>()))
                                  .ThrowsAsync(new System.Exception("Encryption failed"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<System.Exception>(async () =>
            {
                await _patientService.AddPatient(dto);
            });

            Assert.That(ex.Message, Is.EqualTo("Encryption failed"));
        }
    }
}
