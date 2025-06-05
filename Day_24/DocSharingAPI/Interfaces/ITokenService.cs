using DocSharingAPI.Models;

namespace DocSharingAPI.Interfaces
{
    public interface ITokenService
    {
        public Task<string> GenerateToken(User user);
    }
}