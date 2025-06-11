
using AuctionAPI.Exceptions;
using AuctionAPI.Interfaces;
using AuctionAPI.Models;
using AuctionAPI.Models.DTOs;
using Microsoft.Extensions.Logging;

namespace AuctionAPI.Services
{
    public class AuthenticationService : IAuthenticationService
    {

        private readonly ITokenService _tokenService;
        private readonly IRepository<string, User> _userRepository;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly IRepository<string, RefreshToken> _refreshTokenRepository;

        public AuthenticationService(ITokenService tokenService,
                                    IRepository<string, User> userRepository,
                                    IRepository<string, RefreshToken> refreshTokenRepository,
                                    ILogger<AuthenticationService> logger)
        {
            _tokenService = tokenService;
            _userRepository = userRepository;
            _logger = logger;
            _refreshTokenRepository = refreshTokenRepository;
        }
        public async Task<UserLoginResponse> Login(UserLoginRequest user)
        {
            var dbUser = await _userRepository.Get(user.Email.ToLower());
            if (dbUser == null)
            {
                _logger.LogCritical("User not found");
                throw new CollectionEmptyException("No such user");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(user.Password, dbUser.Password);
            if (!isPasswordValid)
            {
                _logger.LogError("Invalid login attempt");
                throw new Exception("Invalid password");
            }

            var token = await _tokenService.GenerateToken(dbUser);
            var refreshToken = await _tokenService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserEmail = dbUser.Email,
                Expires = DateTime.UtcNow.AddDays(7) 
            };

            await _refreshTokenRepository.Add(refreshTokenEntity); 

            return new UserLoginResponse
            {
                Email = user.Email,
                Token = token,
                RefreshToken = refreshToken
            };
        }

        public async Task<RefreshResponse> RefreshTokenAsync(string refreshToken)
        {
            var storedToken = await _refreshTokenRepository.Get(refreshToken);

            if (storedToken == null || storedToken.IsRevoked || storedToken.Expires < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Invalid refresh token");

            var user = await _userRepository.Get(storedToken.UserEmail);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid user");

            var newJwtToken = await _tokenService.GenerateToken(user);

            var newRefreshToken = await _tokenService.GenerateRefreshToken();

            storedToken.IsRevoked = true;
            await _refreshTokenRepository.Update(storedToken.Token, storedToken);

            var refreshTokenEntity = new RefreshToken
            {
                Token = newRefreshToken,
                UserEmail = user.Email,
                Expires = DateTime.UtcNow.AddDays(7),  
                IsRevoked = false
            };
            await _refreshTokenRepository.Add(refreshTokenEntity);
            var result = new RefreshResponse
            {
                AccessToken = newJwtToken,
                RefreshToken = newRefreshToken
            };

            return result;
        }


        public async Task<string> Logout(TokenRefreshRequest token)
        {
            try
            {
                var storedToken = await _refreshTokenRepository.Get(token.RefreshToken);
                if (storedToken == null)
                    throw new CollectionEmptyException("Invalid refresh token");

                storedToken.IsRevoked = true;
                await _refreshTokenRepository.Update(storedToken.Token, storedToken);

                return "Logged Out Successfully";
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
            
        }

    }
}