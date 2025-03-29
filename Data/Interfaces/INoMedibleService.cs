using BackendHackathon2025.Data.Base;
using BackendHackathon2025.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackendHackathon2025.Data.Interfaces
{
    public interface INoMedibleService : IEntityBaseRepository<NoMedible>
	{
        Task<IEnumerable<NoMedible>> GetNoMediblesByUserIdAsync(string userId);
    }
}
