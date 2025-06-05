namespace DocSharingAPI.Models.Dto
{
    public class UploadDocumentDto
    {
        public IFormFile File { get; set; }
        public string Title { get; set; }  
    }

}