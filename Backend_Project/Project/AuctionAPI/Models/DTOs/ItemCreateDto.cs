using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace AuctionAPI.Models.DTOs
{
    public class ItemCreateDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public decimal StartingPrice { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

        [Required]
        public Guid SellerID { get; set; }

        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        public IFormFile? Image { get; set; }
    }
}
