using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendHackathon2025.Data.Models
{
	public class ApplicationUser : IdentityUser
	{

		public DateTime? RefreshTokenExpiryTime { get; set; }
		public string? RefreshToken { get; set; }
		public string? PhoneCode { get; set; }
		public bool IsVerified { get; set; }
		public bool IsActive { get; set; }
		public DateTime? Birthday { get; set; }
		public string? Name { get; set; }
		public string? LastName { get; set; }
	}
}
