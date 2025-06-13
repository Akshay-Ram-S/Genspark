namespace AuctionAPI
{
    public interface IValidation
    {
        public Task<bool> EmailExists(string email);
        public bool IsValidEmail(string email);
        public bool ValidName(string name);
        public Task<bool> ValidAadharAndPAN(string aadhar, string pan);
    }
}