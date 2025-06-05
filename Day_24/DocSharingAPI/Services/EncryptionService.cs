using System.Security.Cryptography;
using System.Text;
using DocSharingAPI.Interfaces;
using DocSharingAPI.Models;

namespace DocSharingAPI.Services
{
    public class EncryptionService : IEncryptionService
    {
        public async Task<EncryptModel> EncryptData(EncryptModel data)
        {
            try
            {
                data.EncryptedData = BCrypt.Net.BCrypt.HashPassword(data.Data);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return data;
        }
    }
}