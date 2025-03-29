using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BackendHackathon2025.Models
{
    public class PersonalizedQuestion
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Category { get; set; }
        
        [Required]
        public string Text { get; set; }
        
        public string Description { get; set; }
        
        public string UserId { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Relación con respuestas
        public virtual ICollection<QuestionResponse> Responses { get; set; }
    }
}
