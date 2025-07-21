
using VideoStream.Models;

namespace VideoStream.Interfaces
{
    public interface IVideoService
    {
        Task<TrainingVideo> UploadVideoAsync(VideoUploadDto dto);
        Task<IEnumerable<TrainingVideo>> GetAllVideosAsync();
        Task<(Stream? Stream, string? FileName)> GetVideoStreamAsync(Guid videoId);
    }
}