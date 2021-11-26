using Newtonsoft.Json;

namespace Studio.Auth.Auth0.Models
{
    public class Auth0PermissionResponse
    {
        [JsonProperty("permission_name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("resource_server_identifier")]
        public string SourceApp { get; set; }
    }
}
