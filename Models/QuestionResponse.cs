using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendHackathon2025.Models
{
    public class QuestionResponse
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Response { get; set; }
        
        public string UserId { get; set; }
        
        [ForeignKey("PersonalizedQuestion")]
        public int QuestionId { get; set; }
        
        public virtual PersonalizedQuestion Question { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
