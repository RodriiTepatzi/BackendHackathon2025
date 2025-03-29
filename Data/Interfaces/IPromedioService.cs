using BackendHackathon2025.Data.Base;
using BackendHackathon2025.Data.Models;

namespace BackendHackathon2025.Data.Interfaces
{
    public interface IPromedioService : IEntityBaseRepository<Promedio>
    {
        Task<IEnumerable<Promedio>> GetPromediosByUserIdAsync(string userId);
    }
}
