using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Studio.Auth.Auth0.Models
{
    public class Auth0UserInfoResponse
    {
        [JsonProperty("name")]
        public string UserName { get; set; }
        [JsonProperty("sub")]
        public string UserId { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("updated_at")]
        public DateTime UpdatedOn { get; set; }
        public List<string> Permissions { get; set; }
        public List<string> Roles { get; set; }
    }
}
