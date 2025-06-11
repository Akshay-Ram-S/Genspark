using AuctionAPI.Interfaces;
using AuctionAPI.Mappers;
using AuctionAPI.Models;
using AuctionAPI.Models.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AuctionAPI.Services
{
    public class SellerService : IUserService<Seller>
    {
        private readonly IFunctionalities _functionalities;
        private readonly UserMapper _mapper;
        private readonly IRepository<string, User> _userRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly IRepository<Guid, Seller> _sellerRepository;
        private readonly IRepository<Guid, Audit> _auditRepository;

        public SellerService(IRepository<string, User> userRepository,
                            IEncryptionService encryptionService,
                            IRepository<Guid, Seller> sellerRepository,
                            IRepository<Guid, Audit> auditRepository,
                            IFunctionalities functionalities)
        {
            _functionalities = functionalities;
            _mapper = new UserMapper();
            _userRepository = userRepository;
            _encryptionService = encryptionService;
            _sellerRepository = sellerRepository;
            _auditRepository = auditRepository; 
        }

        public async Task<Seller> AddUser(AddUserDto user)
        {
            try
            {
                var _user = await _functionalities.RegisterUser(user);
                _user.Role = "Seller";
                _user = await _userRepository.Add(_user);
                var seller = new Seller { UserId = _user.UserId, User = _user};
                seller = await _sellerRepository.Add(seller);
                await _auditRepository.Add(new Audit
                {
                    Action = "Create",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = seller.User.Email,
                    EntityId = seller.UserId,
                    EntityType = "Seller"
                });
                return seller;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<Seller> GetUser(Guid id)
        {
            try
            {
                var user = await _sellerRepository.Get(id);
                if (user == null)
                {
                    throw new Exception($"No seller found with id: {id}");
                }
                return user;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<IEnumerable<Seller>> GetAllUsers(int page, int pageSize)
        {
            try
            {
                var allUsers = await _sellerRepository.GetAll();
                if (allUsers == null)
                {
                    throw new Exception($"No sellers found in the databse");
                }
                var users = allUsers
                    .OrderBy(i => i.User.Name)
                    .ToList();

                var totalCount = users.Count;

                var pagedItems = users
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize);
                    
                return users;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<Seller> DeleteUser(Guid id)
        {
            try
            {
                var user = await _sellerRepository.Get(id);
                if (user == null)
                {
                    throw new Exception($"No seller found with id: {id}");
                }
                var userDel = await _userRepository.Get(user.User.Email);
                userDel.Status = "Deleted";
                userDel = await _userRepository.Update(user.User.Email, userDel);
                await _auditRepository.Add(new Audit
                {
                    Action = "Create",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userDel.Email,
                    EntityId = userDel.UserId,
                    EntityType = "Seller"
                });
                return user;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<Seller> UpdateUser(Guid id, UpdateUserDto user)
        {
            try
            {
                var seller = await _sellerRepository.Get(id); 
                if (seller == null)
                {
                    throw new Exception($"No Seller found with id: {id}");
                }

                var _user = await _userRepository.Get(seller.User.Email); 
                if (user == null)
                {
                    throw new Exception($"User associated with seller ID {id} not found");
                }

                _user.Name = user.Name;
                var encryptedData = await _encryptionService.EncryptData(new EncryptModel
                {
                    Data = user.Password
                });
                _user.Password = encryptedData.EncryptedData;

                await _userRepository.Update(_user.Email, _user); 

                await _auditRepository.Add(new Audit
                {
                    Action = "Update",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = seller.User.Email,
                    EntityId = seller.UserId,
                    EntityType = "Seller"
                });
                return seller;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }

}