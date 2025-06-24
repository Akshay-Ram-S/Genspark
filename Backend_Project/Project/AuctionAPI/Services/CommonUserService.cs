using System.Linq.Expressions;
using AuctionAPI.Interfaces;
using AuctionAPI.Mappers;
using AuctionAPI.Models;
using AuctionAPI.Models.DTOs;

namespace AuctionAPI.Services
{
    public class CommonUserService : ICommonUserService
    {
        private readonly IRepository<string, User> _userRepository;
        private readonly IRepository<Guid, Audit> _auditRepository;
        private readonly IEncryptionService _encryptionService;
        public CommonUserService(IRepository<string, User> userRepository,
                                    IRepository<Guid, Audit> auditRepository,
                                    IEncryptionService encryptionService)
        {
            _userRepository = userRepository;
            _auditRepository = auditRepository;
            _encryptionService = encryptionService;
        }

        public async Task<User> UpdateUser(string email, UpdateUserDto user)
        {
            try
            {
                var updUser = await _userRepository.Get(email);
                if (updUser == null)
                {
                    throw new Exception($"No User found with email: {email}");
                }

                updUser.Name = user.Name;
                var encryptedData = await _encryptionService.EncryptData(new EncryptModel
                {
                    Data = user.Password
                });
                updUser.Password = encryptedData.EncryptedData;

                await _userRepository.Update(email, updUser);

                await _auditRepository.Add(new Audit
                {
                    Action = "Update",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = updUser.Email,
                    EntityId = updUser.UserId,
                    EntityType = "User"
                });
                return updUser;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        public async Task<User> DeleteUser(string email)
        {
            try
            {
                var user = await _userRepository.Get(email);
                if (user == null)
                {
                    throw new Exception($"No User found with email: {email}");
                }
                user.Status = "Deleted";
                user = await _userRepository.Update(email, user);
                await _auditRepository.Add(new Audit
                {
                    Action = "Delete",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = user.Email,
                    EntityId = user.UserId,
                    EntityType = "User"
                });
                return user;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}