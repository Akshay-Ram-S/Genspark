using NUnit.Framework;
using AuctionAPI.Services;
using AuctionAPI.Models;
using System.Threading.Tasks;
using BCrypt.Net;

namespace AuctionAPI.Tests
{
    [TestFixture]
    public class EncryptionServiceTests
    {
        private EncryptionService _encryptionService;

        [SetUp]
        public void Setup()
        {
            _encryptionService = new EncryptionService();
        }

        [Test]
        public async Task EncryptData_ShouldReturnValidBCryptHash()
        {
            // Arrange
            var input = new EncryptModel { Data = "TestPassword123" };

            // Act
            var result = await _encryptionService.EncryptData(input);

            // Assert
            Assert.That(result.EncryptedData, Is.Not.Null.And.Not.Empty, "Encrypted data should not be null or empty");
            Assert.That(result.EncryptedData, Is.Not.EqualTo(input.Data), "Encrypted data should not match input plaintext");
            Assert.That(BCrypt.Net.BCrypt.Verify(input.Data, result.EncryptedData), Is.True, "Encrypted data should be a valid hash of input");
        }

        [TearDown]
        public void TearDown()
        {
            
        }
    }
}
