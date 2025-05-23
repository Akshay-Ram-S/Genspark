using SOLID.interfaces;
using SOLID.models;

namespace SOLID.service
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IUserRepository _userRepository;

        public RegistrationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public bool Register(string username, string password)
        {
            if (_userRepository.UserExists(username))
            {
                Console.WriteLine("Registration failed: Username already taken.");
                return false;
            }

            var user = new User(username, password);
            _userRepository.AddUser(user);
            return true;
        }
    }
}