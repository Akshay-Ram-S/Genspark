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
        

    }

}