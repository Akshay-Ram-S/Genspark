using System.Security.Cryptography;
using System.Text;
using FirstAPI.Interfaces;
using FirstAPI.Models;

namespace FirstAPI.Services
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