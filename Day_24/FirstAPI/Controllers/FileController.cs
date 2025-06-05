using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;

namespace FirstAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly FileExtensionContentTypeProvider _contentTypeProvider;

        public FileController(IWebHostEnvironment environment)
        {
            _environment = environment;
            _contentTypeProvider = new FileExtensionContentTypeProvider();
        }

        [HttpGet("GetFile")]
        public IActionResult GetFile(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return BadRequest("Filename is required.");

            string path = Path.IsPathRooted(filename)? filename : Path.Combine(_environment.WebRootPath, "uploads", filename);

            if (!System.IO.File.Exists(path))
                return NotFound("File not found.");

            if (!_contentTypeProvider.TryGetContentType(path, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var fileBytes = System.IO.File.ReadAllBytes(path);

            return File(fileBytes, contentType);

        }

        [HttpPost("UploadFile")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Use ContentRootPath (app base path) + "uploads" folder
            string uploadPath = Path.Combine(_environment.ContentRootPath, "uploads");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            string fileName = Path.GetFileName(file.FileName);
            string filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { message = "File uploaded successfully!", fileName });
        }
    }
    
}