using SOLID.interfaces;

namespace SOLID.service
{
    public class LoginService : ILoginService
    {
        private readonly IUserRepository _userRepository;

        public LoginService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public bool Login(string username, string password)
        {
            return _userRepository.ValidateUser(username, password);
        }
    }
}