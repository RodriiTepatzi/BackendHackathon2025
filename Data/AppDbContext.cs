using BackendHackathon2025.Data.Models;
using BackendHackathon2025.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BackendHackathon2025.Data
{
	public class AppDbContext : IdentityDbContext<ApplicationUser>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

			base.OnModelCreating(modelBuilder);
		}

		public DbSet<ApplicationUser> ApplicationUsers { get; set; }
		public DbSet<VerificationCode> VerificationCodes { get; set; }
		public DbSet<PersonalizedQuestion> PersonalizedQuestions { get; set; }
		public DbSet<QuestionResponse> QuestionResponses { get; set; }
		public DbSet<Medible> Medibles { get; set; }
		public DbSet<NoMedible> NoMedibles { get; set; }
		public DbSet<Promedio> Promedios { get; set; }

	}
}
