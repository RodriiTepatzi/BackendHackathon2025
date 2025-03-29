using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackendHackathon2025.Data.Base;

namespace BackendHackathon2025.Data.Models
{
	public class NoMedible : IEntityBase
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public string? Id { get; set; }
		public string? UserId { get; set; }
		public string? Clima { get; set; }
		public string? Alimentacion { get; set; }
		public string? Interaccion { get; set; }
		public string? Ocio { get; set; }
		public string? Productividad { get; set; }
		public string? ActividadFisica { get; set; }

		[ForeignKey("UserId")]
		public ApplicationUser? User { get; set; }
	}
}
