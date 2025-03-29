using BackendHackathon2025.Data.Base;
using BackendHackathon2025.Data.Models;

namespace BackendHackathon2025.Data.Interfaces
{
    public interface IMedibleService : IEntityBaseRepository<Medible>
    {
        Task<IEnumerable<Medible>> GetMediblesByUserIdAsync(string userId);
    }
}