using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using TronusDesktop.Models;

namespace TronusDesktop.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public static ApiService Instance { get; } = new ApiService();

        private static bool _isOfflineMode = false;
        public static bool IsOfflineMode
        {
            get => _isOfflineMode;
            set
            {
                if (_isOfflineMode != value)
                {
                    _isOfflineMode = value;
                    OfflineModeChanged?.Invoke(_isOfflineMode);
                }
            }
        }

        public static event Action<bool> OfflineModeChanged;

        private ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://king-blue-nine.vercel.app");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.Timeout = TimeSpan.FromSeconds(8); // Timeout rápido para evitar travamento se a rede estiver instável
        }

        private void ConfigurarAutorizacao()
        {
            if (SessionManager.IsLoggedIn)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SessionManager.AccessToken);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        private async Task TratarErroAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                ApiErrorResponse errorResponse = null;
                try
                {
                    errorResponse = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                }
                catch
                {
                    // If we can't parse the error, just throw with the status code
                }
                
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    SessionManager.Logout();
                }

                throw new ApiException((int)response.StatusCode, errorResponse);
            }
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            if (IsOfflineMode)
            {
                return await OfflineDataService.HandleGetAsync<T>(endpoint);
            }

            ConfigurarAutorizacao();
            var response = await _httpClient.GetAsync(endpoint);
            await TratarErroAsync(response);
            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            if (IsOfflineMode)
            {
                return await OfflineDataService.HandlePostAsync<TRequest, TResponse>(endpoint, data);
            }

            ConfigurarAutorizacao();
            var response = await _httpClient.PostAsJsonAsync(endpoint, data);
            await TratarErroAsync(response);
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task PostAsync<TRequest>(string endpoint, TRequest data)
        {
            if (IsOfflineMode)
            {
                await OfflineDataService.HandlePostAsync(endpoint, data);
                return;
            }

            ConfigurarAutorizacao();
            var response = await _httpClient.PostAsJsonAsync(endpoint, data);
            await TratarErroAsync(response);
        }

        public async Task DeleteAsync(string endpoint)
        {
            if (IsOfflineMode)
            {
                await OfflineDataService.HandleDeleteAsync(endpoint);
                return;
            }

            ConfigurarAutorizacao();
            var response = await _httpClient.DeleteAsync(endpoint);
            await TratarErroAsync(response);
        }
    }
}
