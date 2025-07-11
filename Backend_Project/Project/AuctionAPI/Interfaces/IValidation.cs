namespace AuctionAPI
{
    public interface IValidation
    {
        public Task<bool> EmailExists(string email);
        public Task<bool> PhoneExists(string phone);
        public bool IsValidEmail(string email);
        public bool ValidName(string name);
        public Task<bool> ValidAadharAndPAN(string aadhar, string pan);
        public Task<bool> AadharExists(string aadhar);
        public Task<bool> PanExists(string pan);
    }
}