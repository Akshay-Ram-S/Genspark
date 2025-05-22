using SOLID.models;

namespace SOLID.interfaces
{
    public interface IUserRepository
    {
        void AddUser(User user);
        bool ValidateUser(string username, string password);
        bool UserExists(string username);
    }
}