using DocSharingAPI.Interfaces;
using DocSharingAPI.Models;
using Microsoft.AspNetCore.SignalR;
using DocSharingAPI.Hubs;

namespace DocSharingAPI.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IRepository<int, Document> _documentRepo;
        private readonly IWebHostEnvironment _environment;
        private readonly IHubContext<NotificationHub> _hubContext;

        public DocumentService(IRepository<int, Document> documentRepo,
                                IWebHostEnvironment environment,
                                IHubContext<NotificationHub> hubContext)
        {
            _documentRepo = documentRepo;
            _environment = environment;
            _hubContext = hubContext;
        }

        public async Task<Document> UploadDocumentAsync(IFormFile file, string uploadedBy)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file");

            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                fileBytes = ms.ToArray();
            }

            var document = new Document
            {
                FileName = file.FileName,
                FileContent = fileBytes,
                ContentType = file.ContentType,
                UploadedAt = DateTime.UtcNow
            };

            var savedDoc = await _documentRepo.Add(document);

            await _hubContext.Clients.All.SendAsync("ReceiveNotification", $"📄 New document uploaded by HR: {document.FileName}");

            return savedDoc;
        }

        public async Task<(byte[] FileBytes, string ContentType, string FileName)> GetFileAsync(int id)
        {
            var document = await _documentRepo.Get(id);
            if (document == null || document.FileContent == null)
                throw new FileNotFoundException("File not found in database.");

            return (document.FileContent, document.ContentType ?? "application/octet-stream", document.FileName);
        }


        public async Task<Document> GetDocumentMetadataAsync(int id)
        {
            return await _documentRepo.Get(id);
        }

        public async Task<IEnumerable<Document>> GetAllDocumentsAsync()
        {
            return await _documentRepo.GetAll();
        }
    }
}
