using System.Text.Json.Serialization;

namespace TronusDesktop.Models
{
    public class ResumoDashboardDto
    {
        [JsonPropertyName("total_respostas")]
        public int TotalRespostas { get; set; }

        [JsonPropertyName("total_acertos")]
        public int TotalAcertos { get; set; }

        [JsonPropertyName("taxa_geral")]
        public decimal TaxaGeral { get; set; }

        [JsonPropertyName("total_estrelas")]
        public int TotalEstrelas { get; set; }

        [JsonPropertyName("fases_iniciadas")]
        public int FasesIniciadas { get; set; }

        [JsonPropertyName("fases_completas")]
        public int FasesCompletas { get; set; }
    }

    public class EstatisticaFaseDto
    {
        [JsonPropertyName("reino")]
        public string Reino { get; set; }

        [JsonPropertyName("fase_id")]
        public int FaseId { get; set; }

        [JsonPropertyName("fase")]
        public string Fase { get; set; }

        [JsonPropertyName("tentativas")]
        public int Tentativas { get; set; }

        [JsonPropertyName("acertos")]
        public int Acertos { get; set; }

        [JsonPropertyName("taxa_acerto")]
        public decimal TaxaAcerto { get; set; }

        [JsonPropertyName("estrelas")]
        public int Estrelas { get; set; }
    }
}
