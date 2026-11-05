using System;
using System.Text.Json.Serialization;

namespace TronusDesktop.Models
{
    public class CadastroRequest
    {
        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("celular")]
        public string Celular { get; set; }

        [JsonPropertyName("senha")]
        public string Senha { get; set; }
    }

    public class LoginRequest
    {
        [JsonPropertyName("identificador")]
        public string Identificador { get; set; }

        [JsonPropertyName("senha")]
        public string Senha { get; set; }
    }

    public class UsuarioDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("papel")]
        public string Papel { get; set; }

        [JsonPropertyName("professor_reino_id")]
        public int? ProfessorReinoId { get; set; }

        [JsonPropertyName("criado_em")]
        public string DataCadastro { get; set; }

        [JsonPropertyName("celular")]
        public string Celular { get; set; } = "(11) 98765-4321";

        public string HeroiNome { get; set; }

        public bool Ativo { get; set; } = true;
    }

    public class AtualizarPerfilRequest
    {
        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("celular")]
        public string Celular { get; set; }

        [JsonPropertyName("nome_heroi")]
        public string NomeHeroi { get; set; }

        [JsonPropertyName("genero")]
        public string Genero { get; set; }

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }
    }

    public class AlterarSenhaRequest
    {
        [JsonPropertyName("senha_atual")]
        public string SenhaAtual { get; set; }

        [JsonPropertyName("nova_senha")]
        public string NovaSenha { get; set; }

        [JsonPropertyName("confirmar_senha")]
        public string ConfirmarSenha { get; set; }
    }

    public class EstatisticasAdminDto
    {
        [JsonPropertyName("usuarios")]
        public int Usuarios { get; set; }

        [JsonPropertyName("alunos")]
        public int Alunos { get; set; }

        [JsonPropertyName("personagens")]
        public int Personagens { get; set; }

        [JsonPropertyName("questoes")]
        public int Questoes { get; set; }

        [JsonPropertyName("respostas")]
        public int Respostas { get; set; }

        [JsonPropertyName("taxa_acerto")]
        public decimal TaxaAcerto { get; set; }
    }

    public class AuthResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("usuario")]
        public UsuarioDto Usuario { get; set; }
    }
}
