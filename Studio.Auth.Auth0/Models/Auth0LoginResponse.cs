using Newtonsoft.Json;
using System.Collections.Generic;

namespace Studio.Auth.Auth0.Models
{
    public class Auth0LoginResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonProperty("id_token")]
        public string IdToken { get; set; }
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("scope")]
        public string Scope { get; set; }

        public bool IsAdmin { get; set; }
        public List<string> Permissions { get; set; }
        public List<string> Roles { get; set; }
    }
}
