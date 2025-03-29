using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BackendHackathon2025.Data.Base;

namespace BackendHackathon2025.Data.Models
{
	public class VerificationCode : IEntityBase
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public string? Id { get; set; }
		public string? Code { get; set; }
		public string? UserId { get; set; }

		[ForeignKey("UserId")]
		public ApplicationUser? User { get; set; }
		public DateTime? ExpirationDate { get; set; }
		public DateTime? CreationDate { get; set; }
		public bool IsUsed { get; set; }
	}
}
