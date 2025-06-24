using System.Linq.Expressions;
using AuctionAPI.Interfaces;
using AuctionAPI.Mappers;
using AuctionAPI.Models;
using AuctionAPI.Models.DTOs;

namespace AuctionAPI.Services
{
    public class BidderService : IUserService<Bidder>
    {
        private readonly IRepository<string, User> _userRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly IRepository<Guid, Bidder> _bidderRepository;
        private readonly IRepository<Guid, Audit> _auditRepository;
        private readonly IFunctionalities _functionalities;
        private readonly UserMapper _mapper;

        public BidderService(IRepository<string, User> userRepository,
                            IEncryptionService encryptionService,
                            IRepository<Guid, Bidder> bidderRepository,
                            IRepository<Guid, Audit> auditRepository,
                            IFunctionalities functionalities)
        {
            _functionalities = functionalities;
            _mapper = new UserMapper();
            _userRepository = userRepository;
            _encryptionService = encryptionService;
            _bidderRepository = bidderRepository;
            _auditRepository = auditRepository;
        }

        public async Task<Bidder> AddUser(AddUserDto user)
        {
            try
            {
                var _user = await _functionalities.RegisterUser(user);
                _user.Role = "Bidder";
                var bidder = new Bidder { UserId = _user.UserId, User = _user };
                bidder = await _bidderRepository.Add(bidder);

                await _auditRepository.Add(new Audit
                {
                    Action = "Create",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = bidder.User.Email,
                    EntityId = bidder.UserId,
                    EntityType = "Bidder"
                });
                return bidder;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        public async Task<Bidder> GetUser(Guid id)
        {
            try
            {
                var user = await _bidderRepository.Get(id);
                if (user == null)
                {
                    throw new Exception($"No bidder found with id: {id}");
                }
                return user;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<IEnumerable<Bidder>> GetAllUsers(int page, int pageSize)
        {
            try
            {
                var allUsers = await _bidderRepository.GetAll();
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