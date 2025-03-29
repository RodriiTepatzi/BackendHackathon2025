using BackendHackathon2025.Data.DTOs;
using BackendHackathon2025.Data.Interfaces;
using BackendHackathon2025.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BackendHackathon2025.Controllers
{
    [ApiController]
    [Route("api/v1/nomedibles")]
    public class NoMediblesController : ControllerBase
    {
        private readonly INoMedibleService _noMedibleService;

        public NoMediblesController(INoMedibleService noMedibleService)
        {
            _noMedibleService = noMedibleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNoMedibleData()
        {
            var noMedibleData = await _noMedibleService.GetAllAsync();
            return Ok(noMedibleData);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNoMedibleDataById(string id)
        {
            var noMedibleData = await _noMedibleService.GetByIdAsync(id);
            if (noMedibleData == null) return NotFound();
            
            return Ok(noMedibleData);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetNoMedibleDataByUserId(string userId)
        {
            var noMedibleData = await _noMedibleService.GetNoMediblesByUserIdAsync(userId);
            return Ok(noMedibleData);
        }

        [HttpPost]
        public async Task<IActionResult> AddNoMedibleData([FromBody] NoMedibleDto noMedibleDto)
        {
            var noMedibleEntity = new NoMedible
            {
                Clima = noMedibleDto.Clima,
                Alimentacion = noMedibleDto.Alimentacion,
                Interaccion = noMedibleDto.Interaccion,
                Ocio = noMedibleDto.Ocio,
                Productividad = noMedibleDto.Productividad,
                ActividadFisica = noMedibleDto.ActividadFisica,
                UserId = noMedibleDto.UserId
            };

            await _noMedibleService.AddAsync(noMedibleEntity);
            return CreatedAtAction(nameof(GetNoMedibleDataById), new { id = noMedibleEntity.Id }, noMedibleDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNoMedibleData(string id, [FromBody] NoMedible noMedible)
        {
            if (id != noMedible.Id) return BadRequest();
            
            await _noMedibleService.UpdateAsync(noMedible);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNoMedibleData(string id)
        {
            await _noMedibleService.DeleteAsync(id);
            return NoContent();
        }
    }
}
