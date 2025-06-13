using System.Text.RegularExpressions;
using AuctionAPI.Interfaces;
using AuctionAPI.Models;

namespace AuctionAPI.Validation
{
    public class Validation : IValidation
    {
        private readonly IEncryptionService _encryptionService;
        private readonly IRepository<string, User> _userRepository;

        public Validation(IRepository<string, User> userRepository,
                          IEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
            _userRepository = userRepository;
        }

        public async Task<bool> EmailExists(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return false;

                var users = await _userRepository.GetAll();
                return users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
            }
        }

        public bool IsValidEmail(string email)
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                                        RegexOptions.IgnoreCase | RegexOptions.Compiled);
            return emailRegex.IsMatch(email);
        }

        public bool ValidName(string name)
        {
            if (name.Length < 2)
            {
                throw new Exception("Name must be atleast 2 characters");
            }
            foreach (char ch in name)
            {

            }
            return Regex.IsMatch(name, @"^[A-Za-z ]+$");
        }

        public async Task<bool> ValidAadharAndPAN(string aadhar, string pan)
        {
            if (string.IsNullOrWhiteSpace(pan) || pan.Length != 10)
            {
                throw new Exception("PAN number must be exactly 10 characters long.");
            }

            if (!Regex.IsMatch(pan, @"^[A-Z]{5}\d{4}[A-Z]$"))
            {
                throw new Exception("PAN number must be in the format: 5 letters, 4 digits, and 1 letter.");
            }

            if (string.IsNullOrWhiteSpace(aadhar) || aadhar.Length != 12)
            {
                throw new Exception("Aadhar number must be exactly 12 digits long.");
            }

            if (!Regex.IsMatch(aadhar, @"^\d{12}$"))
            {
                throw new Exception("Aadhar number must contain only digits.");
            }

            return true;
        }

    }
}