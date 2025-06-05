using AutoMapper;
using DocSharingAPI.Interfaces;
using DocSharingAPI.Models;
using DocSharingAPI.Models.DTOs;

namespace DocSharingAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<string, User> _userRepository;
        private readonly IEncryptionService _encryptionService;

        public UserService(IRepository<string, User> userRepository,
                                IEncryptionService encryptionService)

        {
            _userRepository = userRepository;
            _encryptionService = encryptionService;
        }
        
        public async Task<User> AddUser(User user)
        {

            try
            {
                var encryptedData = await _encryptionService.EncryptData(new EncryptModel
                {
                    Data = user.Password
                });
                user.Email = user.Email;
                user.Password = encryptedData.EncryptedData;
                user.Role = user.Role;
                user = await _userRepository.Add(user);
                return user;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }

}