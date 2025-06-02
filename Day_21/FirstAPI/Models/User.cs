using System.ComponentModel.DataAnnotations;

namespace FirstAPI.Models
{


    public class User
    {
        [Key]
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Patient? Patient { get; set; }
        public Doctor? Doctor { get; set; }
    }
}