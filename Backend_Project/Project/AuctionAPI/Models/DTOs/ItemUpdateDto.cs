using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace AuctionAPI.Models.DTOs
{
    public class ItemUpdateDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public DateOnly EndDate { get; set; }
        public string? Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
    }
}
