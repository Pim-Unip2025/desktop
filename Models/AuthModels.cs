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
