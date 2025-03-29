using BackendHackathon2025.Data.Base;
using BackendHackathon2025.Data.Interfaces;
using BackendHackathon2025.Data.Models;

namespace BackendHackathon2025.Data.Services
{
	public class VerificationCodeService : EntityBaseRepository<VerificationCode>, IVerificationCodeService
	{
		public VerificationCodeService(AppDbContext context) : base(context) { }
	}
}
