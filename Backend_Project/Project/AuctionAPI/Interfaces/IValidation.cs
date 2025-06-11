namespace AuctionAPI
{
    public interface IValidation
    {
        public Task<bool> EmailExists(string email);
        public bool ValidName(string name);
        public Task<bool> ValidAadhar(string aadhar);
        public Task<bool> ValidPAN(string pan);
    }
}