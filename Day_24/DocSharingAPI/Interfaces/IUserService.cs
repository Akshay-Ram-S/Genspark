using DocSharingAPI.Models;
namespace DocSharingAPI.Interfaces
{
    public interface IUserService
    {
        public Task<User> AddUser(User user);
    }
}