
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    public class Tweet
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime PostedTime { get; set; }
        public required string UserName { get; set; }

        [ForeignKey("UserName")]
        public User? User { get; set; }

        public ICollection<Like>? Likes { get; set; } 
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Hashtag>? TweetHashtags { get; set; }
    }

}