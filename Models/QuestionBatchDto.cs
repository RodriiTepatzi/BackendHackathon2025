using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BackendHackathon2025.Models
{
    public class QuestionBatchDto
    {
        [JsonPropertyName("Text")]
        public string Text { get; set; }
        
        [JsonPropertyName("Timestamp")]
        public string Timestamp { get; set; }
        
        [JsonPropertyName("AdditionalData")]
        public AdditionalDataDto AdditionalData { get; set; }

        // Acceso directo a las propiedades principales
        [JsonIgnore]
        public string Usuario => AdditionalData?.Usuario;
        
        [JsonIgnore]
        public DatosDto Datos => AdditionalData?.Datos;
    }

    public class AdditionalDataDto
    {
        [JsonPropertyName("usuario")]
        public string Usuario { get; set; }
        
        [JsonPropertyName("datos")]
        public DatosDto Datos { get; set; }
    }

    public class DatosDto
    {
        [JsonPropertyName("preguntas")]
        public List<PreguntaDto> Preguntas { get; set; } = new List<PreguntaDto>();
        
        [JsonPropertyName("respuestas")]
        public List<string> Respuestas { get; set; } = new List<string>();
    }

    public class PreguntaDto
    {
        [JsonPropertyName("categoria")]
        public string Categoria { get; set; }
        
        [JsonPropertyName("pregunta")]
        public string Pregunta { get; set; }
        
        [JsonPropertyName("descripcion")]
        public string Descripcion { get; set; }
    }
}
