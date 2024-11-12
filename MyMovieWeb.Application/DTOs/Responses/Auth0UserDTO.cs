using Newtonsoft.Json;

namespace MyMovieWeb.Application.DTOs.Responses
{
    public class Auth0UserDTO
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("picture")]
        public string Avatar { get; set; }
    }
}
