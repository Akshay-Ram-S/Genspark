using DocSharingAPI.Models;

namespace DocSharingAPI.Interfaces
{
    public interface IDocumentService
    {
        Task<Document> UploadDocumentAsync(IFormFile file, string uploadedBy);
        Task<(byte[] FileBytes, string ContentType, string FileName)> GetFileAsync(int id);
        Task<Document> GetDocumentMetadataAsync(int id);
        Task<IEnumerable<Document>> GetAllDocumentsAsync();

    }
}
