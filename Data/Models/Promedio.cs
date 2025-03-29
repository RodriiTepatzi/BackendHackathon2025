using BackendHackathon2025.Data.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendHackathon2025.Data.Models
{
    public class Promedio : IEntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public float? TemperaturaPromedio { get; set; }
        public float? MovimientoPromedio { get; set; }
        public float? RuidoPromedio { get; set; }
        public float? LuzPromedio { get; set; }
        public DateTime FechaRegistro { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }
}
