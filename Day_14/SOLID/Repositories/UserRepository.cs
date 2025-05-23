using SOLID.interfaces;
using SOLID.models;

namespace SOLID.repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly Dictionary<string, User> _users = new Dictionary<string, User>();

        public void AddUser(User user)
        {
            if (!_users.ContainsKey(user.Username))
            {
                _users.Add(user.Username, user);
                Console.WriteLine("User registered successfully!");
            }
            else
            {
                Console.WriteLine("Username already exists!");
            }
        }

        public bool ValidateUser(string username, string password)
        {
            return _users.ContainsKey(username) && _users[username].Password == password;
        }
        
        public bool UserExists(string username)
        {
            return _users.ContainsKey(username);
        }
    }
}