using AuctionAPI.Models;

namespace AuctionAPI.Interfaces
{
    public interface IEncryptionService
    {
        public Task<EncryptModel> EncryptData(EncryptModel data);
    }
}