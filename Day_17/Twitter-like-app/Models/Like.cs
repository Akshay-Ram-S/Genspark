
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    public class Like
    {
        [Key]
        public int SerialNumber { get; set; }
        public required string UserName { get; set; }
        public int TweetId { get; set; }

        [ForeignKey("UserName")]
        public User? User { get; set; }
        [ForeignKey("TweetId")]
        public Tweet? Tweet { get; set; } 

    }

}