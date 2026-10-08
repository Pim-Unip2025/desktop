using TronusDesktop.Models;

namespace TronusDesktop.Services
{
    public static class SessionManager
    {
        public static string AccessToken { get; private set; }
        public static UsuarioDto CurrentUser { get; private set; }

        public static bool IsLoggedIn => !string.IsNullOrEmpty(AccessToken) && CurrentUser != null;

        public static void Login(AuthResponse authResponse)
        {
            AccessToken = authResponse.AccessToken;
            CurrentUser = authResponse.Usuario;
        }

        public static void Logout()
        {
            AccessToken = null;
            CurrentUser = null;
        }
    }
}
