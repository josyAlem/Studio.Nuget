using Newtonsoft.Json;

namespace Studio.Auth.Auth0.Models
{
    public class Auth0RoleResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
