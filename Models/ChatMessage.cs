using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ConsignmentWebsite.Models
{
    [Table("ChatMessage")]
    public class ChatMessage
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Sender { get; set; } // "User" hoặc "Admin"
        [Required]
        public string Message { get; set; }

        public DateTime SentAt { get; set; } = DateTime.Now;

        [Required]
        public string SessionId { get; set; } // Để phân biệt từng người dùng
    }

}