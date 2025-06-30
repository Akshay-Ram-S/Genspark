using AuctionAPI.Models;
using AuctionAPI.Models.DTOs;

namespace AuctionAPI.Interfaces
{
    public interface ICommonUserService
    {
        public Task<User> UpdateUser(string email, UpdateUserDto user);
        public Task<User> DeleteUser(string email);
        public Task<User> ChangeUserState(UserStatusUpdate statusDto);
        public  Task<User> CreateAdmin(AddUserDto user);

    }
}