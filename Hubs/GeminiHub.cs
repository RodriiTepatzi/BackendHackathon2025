using Microsoft.AspNetCore.SignalR;
using BackendHackathon2025.Data;
using BackendHackathon2025.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace BackendHackathon2025.Hubs
{
	public class GeminiHub : Hub
	{
		private readonly AppDbContext _context;

		public GeminiHub(AppDbContext context)
		{
			_context = context;
		}

		public async Task SendMessage(string user, string message)
		{
			await Clients.All.SendAsync("ReceiveMessage", user, message);
		}

		public async Task ProcessPersonalizedQuestions(string jsonData)
		{
			try
			{
				// Deserializar el JSON recibido
				var questionBatch = JsonSerializer.Deserialize<QuestionBatchDto>(jsonData);
				
				if (questionBatch == null || questionBatch.AdditionalData?.Datos?.Preguntas == null)
				{
					await Clients.Caller.SendAsync("Error", "Formato JSON inválido o datos faltantes");
					return;
				}

				var userId = await GetOrCreateUserIdByName(questionBatch.Usuario);
				
				// Guardar cada pregunta personalizada
				foreach (var pregunta in questionBatch.AdditionalData.Datos.Preguntas)
				{
					var personalizedQuestion = new PersonalizedQuestion
					{
						Category = pregunta.Categoria,
						Text = pregunta.Pregunta,
						Description = pregunta.Descripcion,
						UserId = userId,
						CreatedAt = DateTime.Now
					};
					
					_context.PersonalizedQuestions.Add(personalizedQuestion);
				}
				
				await _context.SaveChangesAsync();
				
				// Notificar éxito al cliente
				await Clients.Caller.SendAsync("QuestionsProcessed", $"Se guardaron {questionBatch.AdditionalData.Datos.Preguntas.Count} preguntas personalizadas");
				
				// Notificar a todos los clientes que hay nuevas preguntas disponibles
				await Clients.All.SendAsync("NewQuestionsAvailable", questionBatch.Usuario);
			}
			catch (Exception ex)
			{
				await Clients.Caller.SendAsync("Error", $"Error al procesar las preguntas: {ex.Message}");
			}
		}
		
		private async Task<string> GetOrCreateUserIdByName(string userName)
		{
			// Buscar usuario por nombre
			var user = await _context.ApplicationUsers
				.FirstOrDefaultAsync(u => u.Name == userName);
				
			if (user != null)
				return user.Id;
				
			// En un caso real, probablemente enviarías un error o manejarías esto de otra manera
			// Este es un enfoque simplificado para el ejemplo
			return userName;
		}

		public async Task SaveQuestionResponse(int questionId, string response, string userId)
		{
			try
			{
				var question = await _context.PersonalizedQuestions.FindAsync(questionId);
				
				if (question == null)
				{
					await Clients.Caller.SendAsync("Error", "Pregunta no encontrada");
					return;
				}
				
				var questionResponse = new QuestionResponse
				{
					QuestionId = questionId,
					Response = response,
					UserId = userId,
					CreatedAt = DateTime.Now
				};
				
				_context.QuestionResponses.Add(questionResponse);
				await _context.SaveChangesAsync();
				
				await Clients.Caller.SendAsync("ResponseSaved", "Respuesta guardada correctamente");
				await Clients.All.SendAsync("NewResponseReceived", userId, questionId);
			}
			catch (Exception ex)
			{
				await Clients.Caller.SendAsync("Error", $"Error al guardar la respuesta: {ex.Message}");
			}
		}
	}
}
