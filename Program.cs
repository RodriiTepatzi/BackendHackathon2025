
using BackendHackathon2025.Data.Models;
using BackendHackathon2025.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BackendHackathon2025.Data.Interfaces;
using BackendHackathon2025.Data.Services;
using DotNetEnv;
using BackendHackathon2025.Hubs;

namespace BackendHackathon2025
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			Env.Load();

			var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

			builder.Services.AddControllers();
			builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString))
				.AddIdentityCore<ApplicationUser>()
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<AppDbContext>();

			builder.Services.AddTransient<IVerificationCodeService, VerificationCodeService>();

			builder.Services.AddOpenApi();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			//builder.Services.AddCors(options =>
			//{
			//	options.AddPolicy("AllowAll",
			//		builder =>
			//		{
			//			builder.WithOrigins("https://localhost:port", "https://backendhackathon202520250328210710.azurewebsites.net")
			//				   .AllowAnyHeader()
			//				   .AllowAnyMethod()
			//				   .AllowCredentials();
			//		});
			//});

			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowAll", builder =>
				{
					builder
						.SetIsOriginAllowed(_ => true) // Accept all origins for testing
													   //.WithOrigins("https://localhost:3000", "https://your-frontend.com") // optional if you want stricter control
						.AllowAnyHeader()
						.AllowAnyMethod()
						.AllowCredentials();
				});
			});


			builder.Services.AddSignalR();

			var app = builder.Build();

			app.UseRouting(); // Setup routing

			app.UseCors("AllowAll");

			app.UseSwagger();
			app.UseSwaggerUI();
			app.MapHub<GeminiHub>("/hubs/gemini"); // SignalR

			if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
