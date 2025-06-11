using System.ComponentModel.DataAnnotations;

namespace AuctionAPI.Models.DTOs
{
    public class UserLoginRequest
    {
        [Required(ErrorMessage = "Email is manditory")]
        public string Email{ get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Password is mandatory")]
        public string Password { get; set; } = string.Empty;
    }
}