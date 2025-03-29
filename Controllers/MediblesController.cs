using BackendHackathon2025.Data.DTOs;
using BackendHackathon2025.Data.Interfaces;
using BackendHackathon2025.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BackendHackathon2025.Controllers
{
    [ApiController]
    [Route("api/v1/medibles")]
    public class MediblesController : ControllerBase
    {
        private readonly IMedibleService _medibleService;

        public MediblesController(IMedibleService medibleService)
        {
            _medibleService = medibleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSensorData()
        {
            var sensorData = await _medibleService.GetAllAsync();
            return Ok(sensorData);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSensorDataById(string id)
        {
            var sensorData = await _medibleService.GetByIdAsync(id);
            if (sensorData == null) return NotFound();
            
            return Ok(sensorData);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetSensorDataByUserId(string userId)
        {
            var sensorData = await _medibleService.GetMediblesByUserIdAsync(userId);
            return Ok(sensorData);
        }

        [HttpPost]
        public async Task<IActionResult> AddSensorData([FromBody] MediblesDto medible)
        {
			var medibleEntity = new Medible
			{
				CantidadDeMovimiento = medible.CantidadDeMovimiento,
				Temperatura = medible.Temperatura,
				FrecuenciaDeRuido = medible.FrecuenciaDeRuido,
				Luz = medible.Luz,
				UserId = medible.UserId
			};



			await _medibleService.AddAsync(medibleEntity);
            return CreatedAtAction(nameof(GetSensorDataById), new { id = medibleEntity.Id }, medible);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSensorData(string id, [FromBody] Medible medible)
        {
            if (id != medible.Id) return BadRequest();
            
            await _medibleService.UpdateAsync(medible);

			return Ok();
		}
	}
}