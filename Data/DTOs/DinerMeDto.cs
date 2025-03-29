namespace BackendHackathon2025.Data.DTOs
{
	public class DinerMeDto
	{
		public string Id { get; set; }
		public string? Email { get; set; }
		public string? UserName { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? PhoneNumber { get; set; }
		public bool IsActive { get; set; }
		public bool IsVerified { get; set; }
		public bool PhoneNumberConfirmed { get; set; }
		public DateTime? Birthday { get; set; }
	}
}
