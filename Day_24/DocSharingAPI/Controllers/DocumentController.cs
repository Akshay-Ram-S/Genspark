using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DocSharingAPI.Interfaces;
using DocSharingAPI.Models;

namespace DocSharingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpPost("UploadFile")]
        [Authorize(Roles = "HRAdmin")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var username = User.Identity?.Name ?? "Unknown"; 
            try
            {
                var result = await _documentService.UploadDocumentAsync(file, username);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetFile/{id}")]
        public async Task<IActionResult> GetFile(int id)
        {
            try
            {
                var (fileBytes, contentType, fileName) = await _documentService.GetFileAsync(id);
                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        [HttpGet("All")]
        public async Task<IActionResult> GetAllDocuments()
        {
            try
            {
                var docs = await _documentService.GetAllDocumentsAsync();
                return Ok(docs);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
