
using DocSharingAPI.Models.DTOs;

namespace DocSharingAPI.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<UserLoginResponse> Login(UserLoginRequest user);
    }
}