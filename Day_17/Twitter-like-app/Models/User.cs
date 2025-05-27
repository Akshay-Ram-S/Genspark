using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    public class User
    {
        [Key]
        public required string UserName { get; set; }
        public string Name { get; set; } = string.Empty;
        public string UserBio { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public ICollection<Tweet>? Tweets { get; set; }
        public ICollection<Like>? Likes { get; set; } 
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<UserFollow>? Followers { get; set; } 
        public ICollection<UserFollow>? Following { get; set; }
    }
}