using BackendHackathon2025.Data.Base;
using BackendHackathon2025.Data.Interfaces;
using BackendHackathon2025.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendHackathon2025.Data.Services
{
    public class MedibleService : EntityBaseRepository<Medible>, IMedibleService
    {
        private readonly AppDbContext _context;
        
        public MedibleService(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Medible>> GetMediblesByUserIdAsync(string userId)
        {
            return await _context.Medibles
                .Where(m => m.UserId == userId)
                .ToListAsync();
        }
    }
}
