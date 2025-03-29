using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BackendHackathon2025.Data;
using BackendHackathon2025.Models;

namespace BackendHackathon2025.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public QuestionController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Question
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonalizedQuestion>>> GetQuestions()
        {
            return await _context.PersonalizedQuestions.ToListAsync();
        }

        // GET: api/Question/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PersonalizedQuestion>> GetQuestion(int id)
        {
            var question = await _context.PersonalizedQuestions.FindAsync(id);

            if (question == null)
            {
                return NotFound();
            }

            return question;
        }

        // GET: api/Question/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<PersonalizedQuestion>>> GetQuestionsByUser(string userId)
        {
            return await _context.PersonalizedQuestions
                .Where(q => q.UserId == userId)
                .ToListAsync();
        }

        // POST: api/Question/batch
        [HttpPost("batch")]
        public async Task<ActionResult<IEnumerable<PersonalizedQuestion>>> CreateQuestions([FromBody] QuestionBatchDto batchDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = batchDto.Usuario;
            var questions = new List<PersonalizedQuestion>();

            foreach (var questionDto in batchDto.Datos.Preguntas)
            {
                var question = new PersonalizedQuestion
                {
                    UserId = userId,
                    Category = questionDto.Categoria,
                    Text = questionDto.Pregunta,
                    Description = questionDto.Descripcion,
                    CreatedAt = DateTime.Now
                };

                _context.PersonalizedQuestions.Add(question);
                questions.Add(question);
            }

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetQuestions), questions);
        }

        // POST: api/Question
        [HttpPost]
        public async Task<ActionResult<PersonalizedQuestion>> CreateQuestion(PersonalizedQuestion question)
        {
            _context.PersonalizedQuestions.Add(question);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetQuestion), new { id = question.Id }, question);
        }

        // POST: api/Question/response
        [HttpPost("response")]
        public async Task<ActionResult<QuestionResponse>> AddResponse(QuestionResponse response)
        {
            _context.QuestionResponses.Add(response);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetResponse", new { id = response.Id }, response);
        }

        // GET: api/Question/response/{id}
        [HttpGet("response/{id}")]
        public async Task<ActionResult<QuestionResponse>> GetResponse(int id)
        {
            var response = await _context.QuestionResponses.FindAsync(id);

            if (response == null)
            {
                return NotFound();
            }

            return response;
        }

        // GET: api/Question/{questionId}/responses
        [HttpGet("{questionId}/responses")]
        public async Task<ActionResult<IEnumerable<QuestionResponse>>> GetResponsesByQuestion(int questionId)
        {
            return await _context.QuestionResponses
                .Where(r => r.QuestionId == questionId)
                .ToListAsync();
        }

        // PUT: api/Question/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion(int id, PersonalizedQuestion question)
        {
            if (id != question.Id)
            {
                return BadRequest();
            }

            _context.Entry(question).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Question/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = await _context.PersonalizedQuestions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            _context.PersonalizedQuestions.Remove(question);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        private bool QuestionExists(int id)
        {
            return _context.PersonalizedQuestions.Any(e => e.Id == id);
        }
    }
}
