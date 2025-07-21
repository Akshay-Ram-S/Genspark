namespace VideoStream.Models
{
    public class VideoUploadDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IFormFile? File { get; set; }
    }
}