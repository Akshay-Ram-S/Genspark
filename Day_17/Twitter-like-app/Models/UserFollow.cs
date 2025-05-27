using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public class UserFollow
    {
        [Key]
        public int SerialNumber { get; set; }
        public string? FollowerUserName { get; set; }
        public string? FollowingUserName { get; set; }

        public User? Follower { get; set; }
        public User? Following { get; set; }
    }

}