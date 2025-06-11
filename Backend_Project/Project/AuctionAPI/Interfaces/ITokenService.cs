using AuctionAPI.Models;
namespace AuctionAPI.Interfaces
{
    public interface ITokenService
    {
        public Task<string> GenerateToken(User user);
        public Task<string> GenerateRefreshToken();
    }
}