using System.Text.Json.Serialization;

namespace TronusDesktop.Models
{
    public class ReinoDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }
    }

    public class ProgressoFaseDto
    {
        [JsonPropertyName("reino_id")]
        public int ReinoId { get; set; }

        [JsonPropertyName("reino_nome")]
        public string ReinoNome { get; set; }

        [JsonPropertyName("fase_id")]
        public int FaseId { get; set; }

        [JsonPropertyName("fase_nome")]
        public string FaseNome { get; set; }

        [JsonPropertyName("ordem")]
        public int Ordem { get; set; }

        [JsonPropertyName("estrelas")]
        public int Estrelas { get; set; }

        [JsonPropertyName("jogada")]
        public bool Jogada { get; set; }
    }
}
