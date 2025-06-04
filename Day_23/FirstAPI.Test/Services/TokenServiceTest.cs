using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirstAPI.Contexts;
using FirstAPI.Models;
using FirstAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;


namespace FirstAPI.Test
{
    public class TokenServiceTests
    {
        private Mock<IConfiguration> _mockConfig = null!;
        private Mock<ClinicContext> _mockContext = null!;
        private Mock<DbSet<Doctor>> _mockDoctorSet = null!;
        private TokenService _tokenService = null!;

        [SetUp]
        public void Setup()
        {
            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.Setup(c => c["Keys:JwtTokenKey"]).Returns("secret_tokn");

            var doctors = new List<Doctor>
            {
                new Doctor { Id = 1, Email = "doctor@gmail.com", YearsOfExperience = 10, Name = "test123" }
            }.AsQueryable();

            _mockDoctorSet = new Mock<DbSet<Doctor>>();
            _mockDoctorSet.As<IQueryable<Doctor>>().Setup(m => m.Provider).Returns(doctors.Provider);
            _mockDoctorSet.As<IQueryable<Doctor>>().Setup(m => m.Expression).Returns(doctors.Expression);
            _mockDoctorSet.As<IQueryable<Doctor>>().Setup(m => m.ElementType).Returns(doctors.ElementType);
            _mockDoctorSet.As<IQueryable<Doctor>>().Setup(m => m.GetEnumerator()).Returns(doctors.GetEnumerator());

            _mockContext = new Mock<ClinicContext>(new DbContextOptions<ClinicContext>());
            _mockContext.Setup(c => c.Doctors).Returns(_mockDoctorSet.Object);

            _tokenService = new TokenService(_mockConfig.Object, _mockContext.Object);
        }

        [Test]
        public async Task GenerateToken_ForDoctor_IncludesYearsOfExperienceClaim()
        {
            // Arrange
            var user = new User
            {
                Username = "doctor@gmail.com",
                Role = "Doctor"
            };

            // Act
            var token = await _tokenService.GenerateToken(user);

            // Assert
            Assert.IsNotNull(token);
            Assert.IsFalse(string.IsNullOrEmpty(token));

            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            Assert.IsTrue(jwtToken.Claims.Any(c => c.Type == "YearsOfExperience" && c.Value == "10"));
            Assert.IsTrue(jwtToken.Claims.Any(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier && c.Value == user.Username));
            Assert.IsTrue(jwtToken.Claims.Any(c => c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == user.Role));

        }

        [Test]
        public async Task GenerateToken_ForNonDoctor_DoesNotIncludeYearsOfExperienceClaim()
        {
            // Arrange
            var user = new User
            {
                Username = "test@gmail.com",
                Role = "Patient"
            };

            // Act
            var token = await _tokenService.GenerateToken(user);

            // Assert
            Assert.IsNotNull(token);
            Assert.IsFalse(string.IsNullOrEmpty(token));


            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            Assert.IsFalse(jwtToken.Claims.Any(c => c.Type == "YearsOfExperience"));
            Assert.IsTrue(jwtToken.Claims.Any(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier && c.Value == user.Username));
            Assert.IsTrue(jwtToken.Claims.Any(c => c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == user.Role));
        }
    }
}
