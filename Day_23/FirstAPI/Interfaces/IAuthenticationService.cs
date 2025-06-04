
using FirstAPI.Models.DTOs.DoctorSpecialities;

namespace FirstAPI.Interfaces
{
    public interface IAuthenticationService
    {
        //public Task<UserLoginResponse> Login(UserLoginRequest user);
        public Task<UserLoginResponse> LoginWithGoogle(string idToken);


    }
}