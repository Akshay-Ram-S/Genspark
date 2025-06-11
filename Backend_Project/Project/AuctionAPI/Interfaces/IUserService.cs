using System.Linq.Expressions;
using AuctionAPI.Models;
using AuctionAPI.Models.DTOs;

namespace AuctionAPI.Interfaces
{
    public interface IUserService<T> where T : class
    {
        public Task<T> AddUser(AddUserDto user);
        public Task<T> GetUser(Guid id);
        public Task<IEnumerable<T>> GetAllUsers(int page, int pageSize);
        public Task<T> UpdateUser(Guid id, UpdateUserDto user);
        public Task<T> DeleteUser(Guid id);

    }
}