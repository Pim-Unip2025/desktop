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

        // Make it a singleton or instantiate once in Program.cs. For simplicity, we use a single instance.
        public static ApiService Instance { get; } = new ApiService();

        private ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://king-blue-nine.vercel.app");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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
                
                // If 401 Unauthorized, we might want to trigger a global event to logout
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    SessionManager.Logout();
                    // We can handle the redirect to login screen in the UI layer catching this exception
                }

                throw new ApiException((int)response.StatusCode, errorResponse);
            }
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            ConfigurarAutorizacao();
            var response = await _httpClient.GetAsync(endpoint);
            await TratarErroAsync(response);
            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            ConfigurarAutorizacao();
            var response = await _httpClient.PostAsJsonAsync(endpoint, data);
            await TratarErroAsync(response);
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task PostAsync<TRequest>(string endpoint, TRequest data)
        {
            ConfigurarAutorizacao();
            var response = await _httpClient.PostAsJsonAsync(endpoint, data);
            await TratarErroAsync(response);
        }

        public async Task DeleteAsync(string endpoint)
        {
            ConfigurarAutorizacao();
            var response = await _httpClient.DeleteAsync(endpoint);
            await TratarErroAsync(response);
        }
    }
}
