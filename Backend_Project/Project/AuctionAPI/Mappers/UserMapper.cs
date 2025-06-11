
using AuctionAPI.Models;
using AuctionAPI.Models.DTOs;

namespace AuctionAPI.Mappers
{
    public class UserMapper
    {
        public User? MapUser(AddUserDto user)
        {
            User newUser = new();
            newUser.Name = user.Name;
            newUser.Email = user.Email.ToLower();
            newUser.Aadhar = user.Aadhar;
            newUser.PAN = user.PAN.ToUpper();
            newUser.Status = "Active";
            return newUser;
        }
    }
}