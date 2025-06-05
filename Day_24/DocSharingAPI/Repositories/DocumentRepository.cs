using DocSharingAPI.Contexts;
using DocSharingAPI.Interfaces;
using DocSharingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DocSharingAPI.Repositories
{
    public  class DocumentRepository : Repository<int, Document>
    {
        public DocumentRepository(DocumentContext documentContext) : base(documentContext)
        {
        }

        public override async Task<Document> Get(int key)
        {
            var document = await _documentContext.Documents.SingleOrDefaultAsync(p => p.Id == key);

            return document??throw new Exception("No document with the given ID");
        }

        public override async Task<IEnumerable<Document>> GetAll()
        {
            var documents = _documentContext.Documents;
            if (documents.Count() == 0)
                throw new Exception("No document in the database");
            return (await documents.ToListAsync());
        }
    }
}