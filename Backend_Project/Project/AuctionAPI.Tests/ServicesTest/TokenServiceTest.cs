using FirstAPI.Services;
using AuctionAPI.Models;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;

namespace AuctionAPI.Tests
{
    [TestFixture]
    public class TokenServiceTests
    {
        private TokenService _tokenService = null!;
        private IConfiguration _configuration = null!;

        [SetUp]
        public void SetUp()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                { "Keys:JwtTokenKey", "svudvjsbhvsyusfdyafydgjbisgdywfbscgufcwgcigw" }
            };
            _configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings!).Build();
            _tokenService = new TokenService(_configuration);
        }


        [Test]
        public async Task GenerateRefreshToken_ShouldReturnBase64String()
        {
            var token = await _tokenService.GenerateRefreshToken();

            Assert.That(token, Is.Not.Null.And.Not.Empty);
            Assert.DoesNotThrow(() => Convert.FromBase64String(token));
        }

        [Test]
        public void Constructor_ShouldThrowException_WhenKeyIsMissing()
        {
            var badConfig = new ConfigurationBuilder().AddInMemoryCollection().Build();

            var ex = Assert.Throws<ArgumentNullException>(() => new TokenService(badConfig));
            Assert.That(ex!.Message, Does.Contain("Value cannot be null"));
        }

        [Test]
        public void GenerateToken_ShouldThrowException_WhenUserIsNull()
        {
            Assert.ThrowsAsync<NullReferenceException>(async () => await _tokenService.GenerateToken(null!));
        }
    }
}
