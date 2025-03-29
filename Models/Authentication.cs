using System.ComponentModel.DataAnnotations;

namespace BackendHackathon2025.Models
{
	public class ResendVerificationModel
	{
		public string PhoneNumber { get; set; } = string.Empty;
		public string PhoneCode { get; set; } = string.Empty;
	}

	public class CheckPhoneModel
	{
		public string PhoneNumber { get; set; } = string.Empty;
		public string PhoneCode { get; set; } = string.Empty;
	}

	public class VerifyPhoneModel
	{
		public string PhoneCode { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
		public string VerificationCode { get; set; } = string.Empty;
	}

	public class RegisterModel
	{
		[Required]
		public string? Email { get; set; }

		[Required]
		public string? Password { get; set; }

		[Required]
		public string? Name { get; set; }

		[Required]
		public string? LastName { get; set; }

		[Required]
		public string? PhoneNumber { get; set; }

		[Required]
		public string? PhoneCode { get; set; }

		[Required]
		public string? VerificationCode { get; set; }

		[Required]
		public DateTime Birthday { get; set; }
	}

	public class LoginModel
	{
		[Required]
		public string? PhoneNumber { get; set; }

		[Required]
		public string? PhoneCode { get; set; }

		[Required]
		public string? Password { get; set; }

		[Required]
		public bool RememberMe { get; set; }
	}

	public class TokenRefreshModel
	{
		[Required]
		public string? AccessToken { get; set; }

		[Required]
		public string? RefreshToken { get; set; }

		[Required]
		public DateTime? ExpiryDate { get; set; }
	}

	public class ChangePasswordModel
	{
		[Required]
		public string? OldPassword { get; set; }
		[Required]
		public string? NewPassword { get; set; }
	}
}
