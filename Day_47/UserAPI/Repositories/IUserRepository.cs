using UserApiApp.Models;

namespace UserApiApp.Repository;

public interface IUserRepository
{
    Task<User> AddUserAsync(User user);
}
