using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TronusDesktop.Models
{
    public class AlternativaDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("letra")]
        public string Letra { get; set; }

        [JsonPropertyName("texto")]
        public string Texto { get; set; }

        [JsonPropertyName("correta")]
        public bool Correta { get; set; }
    }

    public class QuestaoDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("fase_id")]
        public int FaseId { get; set; }

        [JsonPropertyName("enunciado")]
        public string Enunciado { get; set; }

        [JsonPropertyName("nivel_dificuldade")]
        public string NivelDificuldade { get; set; }

        [JsonPropertyName("alternativas")]
        public List<AlternativaDto> Alternativas { get; set; } = new();
    }

    public class ItemRespostaRequest
    {
        [JsonPropertyName("questaoId")]
        public int QuestaoId { get; set; }

        [JsonPropertyName("alternativaId")]
        public int AlternativaId { get; set; }
    }

    public class EnviarRespostasRequest
    {
        [JsonPropertyName("respostas")]
        public List<ItemRespostaRequest> Respostas { get; set; } = new();
    }

    public class ItemCorrecaoDto
    {
        [JsonPropertyName("questao_id")]
        public int QuestaoId { get; set; }

        [JsonPropertyName("alternativa_escolhida_id")]
        public int AlternativaEscolhidaId { get; set; }

        [JsonPropertyName("alternativa_correta_id")]
        public int AlternativaCorretaId { get; set; }

        [JsonPropertyName("acertou")]
        public bool Acertou { get; set; }
    }

    public class ResultadoQuizResponse
    {
        [JsonPropertyName("fase_id")]
        public int FaseId { get; set; }

        [JsonPropertyName("acertos")]
        public int Acertos { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("estrelas")]
        public int Estrelas { get; set; }

        [JsonPropertyName("melhor_estrelas")]
        public int MelhorEstrelas { get; set; }

        [JsonPropertyName("personagem")]
        public PersonagemDto Personagem { get; set; }

        [JsonPropertyName("correcao")]
        public List<ItemCorrecaoDto> Correcao { get; set; } = new();
    }
}
