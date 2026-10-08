using System.Text.Json.Serialization;

namespace TronusDesktop.Models
{
    public class CriarPersonagemRequest
    {
        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("genero")]
        public string Genero { get; set; }

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }
    }

    public class PersonagemDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("genero")]
        public string Genero { get; set; }

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }

        [JsonPropertyName("titulo_atual")]
        public string TituloAtual { get; set; }

        [JsonPropertyName("total_estrelas")]
        public int TotalEstrelas { get; set; }
    }
}
