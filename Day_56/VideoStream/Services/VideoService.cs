using Azure.Storage.Blobs;
using VideoStream.Interfaces;
using VideoStream.Models;
using VideoStream.Repositories;

namespace VideoStream.Services
{
    public class VideoService : IVideoService
    {
        private readonly BlobContainerClient _containerClient;
        private readonly IRepository<Guid,TrainingVideo> _videoRepository;

        public VideoService(IConfiguration configuration, IRepository<Guid,TrainingVideo> videoRepository)
        {
            var sasUrl = configuration["AzureBlob:SasUrl"];
            _containerClient = new BlobContainerClient(new Uri(sasUrl));
            _videoRepository = videoRepository;
        }

        public async Task<TrainingVideo> UploadVideoAsync(VideoUploadDto dto)
        {
            if (dto.File == null || dto.File.Length == 0)
                throw new Exception("File is required.");

            var fileName = dto.File.FileName;

            await using var stream = dto.File.OpenReadStream();
            var blobClient = _containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(stream, overwrite: true);

            var video = new TrainingVideo
            {
                Title = dto.Title,
                Description = dto.Description,
                UploadDate = DateTime.UtcNow,
                BlobUrl = blobClient.Uri.ToString()
            };

            await _videoRepository.Add(video);

            return video;
        }

        public async Task<IEnumerable<TrainingVideo>> GetAllVideosAsync()
        {
            return await _videoRepository.GetAll();
        }

        public async Task<(Stream? Stream, string? FileName)> GetVideoStreamAsync(Guid videoId)
        {
            var video = await _videoRepository.Get(videoId);
            if (video == null) return (null, null);

            var uri = new Uri(video.BlobUrl);
            var fileName = Path.GetFileName(uri.AbsolutePath);
            var blobClient = _containerClient.GetBlobClient(fileName);

            if (!await blobClient.ExistsAsync())
                return (null, null);

            var response = await blobClient.DownloadStreamingAsync();
            return (response.Value.Content, fileName);
        }
    }
}
