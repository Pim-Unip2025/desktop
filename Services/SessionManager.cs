using TronusDesktop.Models;

namespace TronusDesktop.Services
{
    public static class SessionManager
    {
        public static string AccessToken { get; private set; }
        public static UsuarioDto CurrentUser { get; private set; }
        public static PersonagemDto CachedHero { get; set; }
        public static string CurrentPassword { get; set; } = "123456";

        public static bool IsLoggedIn => !string.IsNullOrEmpty(AccessToken) && CurrentUser != null;

        public static void Login(AuthResponse authResponse, string senhaInformada = null)
        {
            AccessToken = authResponse.AccessToken;
            CurrentUser = authResponse.Usuario;
            CachedHero = null;
            if (!string.IsNullOrWhiteSpace(senhaInformada))
            {
                CurrentPassword = senhaInformada;
            }
        }

        public static void UpdateUserProfile(string nome, string email, string celular, PersonagemDto updatedHero = null)
        {
            if (CurrentUser != null)
            {
                if (!string.IsNullOrWhiteSpace(nome)) CurrentUser.Nome = nome;
                if (!string.IsNullOrWhiteSpace(email)) CurrentUser.Email = email;
                if (!string.IsNullOrWhiteSpace(celular)) CurrentUser.Celular = celular;
                if (updatedHero != null)
                {
                    CurrentUser.HeroiNome = $"{updatedHero.Nome} ({updatedHero.TituloAtual} · {updatedHero.TotalEstrelas}★)";
                }
            }
            if (updatedHero != null)
            {
                CachedHero = updatedHero;
            }
        }

        public static void Logout()
        {
            AccessToken = null;
            CurrentUser = null;
            CachedHero = null;
        }
    }
}
