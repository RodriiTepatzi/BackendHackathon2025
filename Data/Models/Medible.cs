using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BackendHackathon2025.Data.Base;

namespace BackendHackathon2025.Data.Models
{
	public class Medible : IEntityBase
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public string? Id { get; set; }
		public string? UserId { get; set; }
		public float? Temperatura { get; set; }
		public float? CantidadDeMovimiento { get; set; }
		public float? FrecuenciaDeRuido { get; set; }
		public float? Luz { get; set; }

		// Optional: Navigation property
		public ApplicationUser? User { get; set; }
	}

}
