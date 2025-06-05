using System.ComponentModel.DataAnnotations;

namespace DocSharingAPI.Models
{
    public class User
    {
        [Key]
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }

}