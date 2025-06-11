using AuctionAPI.Models.DTOs;
namespace AuctionAPI.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<UserLoginResponse> Login(UserLoginRequest user);
        public Task<string> Logout(TokenRefreshRequest token);
        public Task<RefreshResponse> RefreshTokenAsync(string refreshToken);
    }
}