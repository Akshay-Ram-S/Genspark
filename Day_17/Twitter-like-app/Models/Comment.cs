
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime PostedAt { get; set; }
        public required string UserName { get; set; }
        public int TweetId { get; set; }

        [ForeignKey("UserName")]
        public User? User { get; set; }
        [ForeignKey("TweetId")]
        public Tweet? Tweet { get; set; } 
    }

}