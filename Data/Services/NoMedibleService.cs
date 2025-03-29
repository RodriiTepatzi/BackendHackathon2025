using BackendHackathon2025.Data.Base;
using BackendHackathon2025.Data.Interfaces;
using BackendHackathon2025.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendHackathon2025.Data.Services
{
    public class NoMedibleService : EntityBaseRepository<NoMedible>, INoMedibleService
    {
        private readonly AppDbContext _context;
        
        public NoMedibleService(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NoMedible>> GetNoMediblesByUserIdAsync(string userId)
        {
            return await _context.NoMedibles
                .Where(nm => nm.UserId == userId)
                .ToListAsync();
        }
	}
}
