using Microsoft.AspNetCore.Mvc;
using AuctionAPI.Contexts;
using AuctionAPI.Mappers;

namespace AuctionAPI.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly AuctionContext _context;
        private readonly ILogger<ImageController> _logger;

        public ImageController(ILogger<ImageController> logger,
                               AuctionContext context)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("view/{itemId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ViewImage(Guid itemId)
        {
            var user = User?.Identity?.Name ?? "Anonymous";

            _logger.LogInformation($"User {User} called GET /api/v1/image/view/{itemId}");

            try
            {
                var itemDetails = await _context.ItemDetails.FindAsync(itemId);

                if (itemDetails == null)
                {
                    _logger.LogWarning("User {User} requested image for nonexistent item {ItemId}", user, itemId);
                    return NotFound(ApiResponseMapper.NotFound<string>("Item not found"));
                }

                if (itemDetails.ImageData == null)
                {
                    _logger.LogWarning("User {User} requested image for item {ItemId} with no image data", user, itemId);
                    return NotFound(ApiResponseMapper.NotFound<string>("No image uploaded for given ID"));
                }

                _logger.LogInformation("Image successfully retrieved for item {ItemId} by user {User}", itemId, user);

                return File(itemDetails.ImageData, itemDetails.ImageMimeType ?? "image/jpeg");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving image for item {ItemId} by user {User}", itemId, user);
                return StatusCode(500, ApiResponseMapper.InternalError<string>(ex));
            }
        }
    }
}
