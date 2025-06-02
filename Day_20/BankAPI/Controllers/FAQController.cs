// Controllers/ChatController.cs
using Microsoft.AspNetCore.Mvc;
using BankAPI.Services;
using BankAPI.Models.Dtos;

namespace BankAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class FAQController : ControllerBase
    {
        private readonly FAQService _FAQService;

        public FAQController(FAQService FAQService)
        {
            _FAQService = FAQService;
        }

        [HttpPost("FAQ")]
        public IActionResult Ask([FromBody] QuestionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
            {
                return BadRequest("Please re-enter input. You have given no input.");
            }

            var answer = _FAQService.GetResponse(request.Question);
            return Ok(new { answer });
        }

        [HttpGet("FAQ/history")]
        public IActionResult GetChatHistory()
        {
            var history = _FAQService.GetChatHistory();
            return Ok(history);
        }
    }
}
