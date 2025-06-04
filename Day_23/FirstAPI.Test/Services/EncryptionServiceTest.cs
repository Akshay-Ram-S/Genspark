using System;
using System.Threading.Tasks;
using FirstAPI.Models;
using FirstAPI.Services;
using NUnit.Framework;

namespace FirstAPI.Test
{
    public class EncryptionServiceTests
    {
        private EncryptionService _encryptionService;

        [SetUp]
        public void Setup()
        {
            _encryptionService = new EncryptionService();
        }

        [Test]
        public async Task EncryptData_ReturnsHashedData_ForValidInput()
        {
            // Arrange
            var input = new EncryptModel
            {
                Data = "qwerty"
            };

            // Act
            var result = await _encryptionService.EncryptData(input);

            // Assert
            Assert.That(result.EncryptedData, Is.Not.Null.And.Not.Empty);
            Assert.That(result.EncryptedData, Is.Not.EqualTo("qwerty")); 
            Assert.That(BCrypt.Net.BCrypt.Verify("qwerty", result.EncryptedData), Is.True); 
        }

        [Test]
        public void EncryptData_ThrowsException_WhenDataIsNull()
        {
            // Arrange
            var input = new EncryptModel
            {
                Data = null 
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
            {
                await _encryptionService.EncryptData(input);
            });

            Assert.That(ex.Message, Is.Not.Null.And.Not.Empty);
        }
    }
}
