using BackendHackathon2025.Data.Base;
using BackendHackathon2025.Data.Interfaces;
using BackendHackathon2025.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendHackathon2025.Data.Services
{
    public class PromedioService : EntityBaseRepository<Promedio>, IPromedioService
    {
        private readonly AppDbContext _context;
        
        public PromedioService(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Promedio>> GetPromediosByUserIdAsync(string userId)
        {
            return await _context.Promedios
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }
    }
}
