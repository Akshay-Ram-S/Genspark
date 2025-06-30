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
        private readonly IFunctionalities _functionalities;

        public CommonUserService(IRepository<string, User> userRepository,
                                    IRepository<Guid, Audit> auditRepository,
                                    IEncryptionService encryptionService,
                                    IFunctionalities functionalities)
        {
            _userRepository = userRepository;
            _auditRepository = auditRepository;
            _encryptionService = encryptionService;
            _functionalities = functionalities;
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

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(user.CurrentPassword, updUser.Password);
                if (!isPasswordValid)
                {
                    throw new Exception("Incorrect password");
                }
                var encryptedData = await _encryptionService.EncryptData(new EncryptModel
                {
                    Data = user.NewPassword
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

        public async Task<User> ChangeUserState(UserStatusUpdate statusDto)
        {
            try
            {
                var user = await _userRepository.Get(statusDto.Email);
                if (user == null)
                {
                    throw new Exception($"No User found with email: {statusDto.Email}");
                }
                user.Status = statusDto.Status;
                user = await _userRepository.Update(user.Email, user);
                await _auditRepository.Add(new Audit
                {
                    Action = "Disable",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "Admin",
                    EntityId = new Guid(),
                    EntityType = "User"
                });
                return user;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<User> CreateAdmin(AddUserDto user)
        {
            try
            {
                var newUser = await _functionalities.RegisterUser(user);
                newUser.Role = "Admin";
                var admin = await _userRepository.Add(newUser);
                return admin;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}