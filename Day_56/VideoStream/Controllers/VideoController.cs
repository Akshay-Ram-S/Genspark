using Microsoft.AspNetCore.Mvc;
using VideoStream.Interfaces;
using VideoStream.Models;

namespace VideoStream.Controllers
{
    [ApiController]
    [Route("api/videos")]
    public class VideosController : ControllerBase
    {
        private readonly IVideoService _videoService;

        public VideosController(IVideoService videoService)
        {
            _videoService = videoService;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] VideoUploadDto dto)
        {
            try
            {
                var video = await _videoService.UploadVideoAsync(dto);
                return Ok(video);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading video: {ex.Message}");
                return StatusCode(500, "Internal server error while uploading the video.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var videos = await _videoService.GetAllVideosAsync();
                var result = videos.Select(v => new { v.Id, v.Title, v.Description, v.BlobUrl });
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching videos: {ex.Message}");
                return StatusCode(500, "Internal server error while fetching videos.");
            }
        }

        [HttpGet("{id}/stream")]
        public async Task<IActionResult> StreamVideo(Guid id)
        {
            try
            {
                var (stream, fileName) = await _videoService.GetVideoStreamAsync(id);
                if (stream == null || fileName == null)
                    return NotFound("Video or blob not found.");

                var contentType = GetContentType(fileName);
                return File(stream, contentType, enableRangeProcessing: true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error streaming video: {ex.Message}");
                return StatusCode(500, "Internal server error while streaming the video.");
            }
        }

        private string GetContentType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".mp4" => "video/mp4",
                ".webm" => "video/webm",
                ".mov" => "video/quicktime",
                ".ogg" => "video/ogg",
                ".mp3" => "audio/mpeg",
                ".wav" => "audio/wav",
                _ => "application/octet-stream"
            };
        }
    }
}
