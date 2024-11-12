using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Application.Interfaces;
using Newtonsoft.Json;
using System.Text;

namespace MyMovieWeb.Application.Services
{
    public class Auth0Services : IAuth0Services
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;

        public Auth0Services(IConfiguration configuration, HttpClient httpClient, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _memoryCache = memoryCache;
        }

        public async Task<string?> GetAccessToken()
        {
            string? clientId = _configuration["Auth0Management:ClientId"];
            string? clientSecret = _configuration["Auth0Management:ClientSecret"];
            string? audience = _configuration["Auth0Management:Audience"];

            string? requestUrl = _configuration["Auth0Management:TokenUrl"];

            var requestBody = new Dictionary<string, string?>
            {
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "audience", audience },
                { "grant_type", "client_credentials" }
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(requestUrl, requestContent);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to get auth0 access token");
            }

            string? responseData = await response.Content.ReadAsStringAsync();

            var tokenResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseData);

            return tokenResponse["access_token"];
        }

        public async Task<Auth0UserDTO?> GetUser(string userId)
        {
            if (_memoryCache.TryGetValue($"Auth0User_{userId}", out Auth0UserDTO? cachedUser))
            {
                return cachedUser;
            }
            string accessToken = await GetAccessToken();
            string requestUrl = $"{_configuration["Auth0Management:Audience"]}users/{userId}";

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl)
            {
                Headers = { Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken) }
            };

            var response = await _httpClient.SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to fetch users from Auth0: {response.StatusCode}");
            }

            string responseData = await response.Content.ReadAsStringAsync();

            var user = JsonConvert.DeserializeObject<Auth0UserDTO>(responseData);

            _memoryCache.Set($"Auth0User_{userId}", user, TimeSpan.FromHours(1));

            return user;
        }


        public async Task<List<Auth0UserDTO>?> GetAllUsers()
        {
            if (_memoryCache.TryGetValue("Auth0Users", out List<Auth0UserDTO>? cachedUsers))
            {
                return cachedUsers;
            }
            string accessToken = await GetAccessToken();
            string requestUrl = $"{_configuration["Auth0Management:Audience"]}users";

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl)
            {
                Headers = { Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken) }
            };

            var response = await _httpClient.SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to fetch users from Auth0: {response.StatusCode}");
            }

            string responseData = await response.Content.ReadAsStringAsync();

            var users = JsonConvert.DeserializeObject<List<Auth0UserDTO>>(responseData);

            _memoryCache.Set("Auth0Users", users, TimeSpan.FromHours(1));

            return users;
        }

    }
}
