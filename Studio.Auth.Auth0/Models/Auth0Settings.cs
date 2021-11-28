using System.Text.Json.Serialization;

namespace Studio.Auth.Auth0.Models
{

    public class Paths
    {
        [JsonPropertyName("Token")]
        public string Token { get; set; }

        [JsonPropertyName("Auth")]
        public string Auth { get; set; }

        [JsonPropertyName("DeviceAuth")]
        public string DeviceAuth { get; set; }

        [JsonPropertyName("UserInfo")]
        public string UserInfo { get; set; }
    }

    public class GrantTypes
    {
        [JsonPropertyName("Client")]
        public string Client { get; set; }
        [JsonPropertyName("Refresh")]
        public string Refresh { get; set; }

        [JsonPropertyName("Password")]
        public string Password { get; set; }

        [JsonPropertyName("PasswordRealm")]
        public string PasswordRealm { get; set; }
    }

    public class Token : PathType
    {
     
    }

    public class Users : PathType
    {
       
    }

    public class Roles: PathType
    {
       
    }
    public class Permissions: PathType
    {
       
    }

    public class ManagementAPI
    {
        [JsonPropertyName("Token")]
        public Token Token { get; set; }

        [JsonPropertyName("Signup")]
        public Users Users { get; set; }

        [JsonPropertyName("Roles")]
        public Roles Roles { get; set; }
        [JsonPropertyName("Permissions")]
        public Permissions Permissions { get; set; }
    }

    public class AuthAPI
    {
        [JsonPropertyName("ClientId")]
        public string ClientId { get; set; }

        [JsonPropertyName("ClientSecret")]
        public string ClientSecret { get; set; }

        [JsonPropertyName("Audience")]
        public string Audience { get; set; }

        [JsonPropertyName("Connectionrealm")]
        public string ConnectionRealm { get; set; }
        [JsonPropertyName("Scope")]
        public string Scope { get; set; }
        [JsonPropertyName("AdminRole")]
        public string AdminRole { get; set; }
    }

    public class ClientSPA
    {
        [JsonPropertyName("ClientId")]
        public string ClientId { get; set; }

        [JsonPropertyName("ClientSecret")]
        public string ClientSecret { get; set; }
    }

    public class Auth0Settings
    {
        [JsonPropertyName("Domain")]
        public string Domain { get; set; }

        [JsonPropertyName("Paths")]
        public Paths Paths { get; set; }

        [JsonPropertyName("GrantTypes")]
        public GrantTypes GrantTypes { get; set; }

        [JsonPropertyName("ManagementAPI")]
        public ManagementAPI ManagementAPI { get; set; }

        [JsonPropertyName("AuthAPI")]
        public AuthAPI AuthAPI { get; set; }

        [JsonPropertyName("ClientSPA")]
        public ClientSPA ClientSPA { get; set; }
    }

    public class PathType {
        [JsonPropertyName("Method")]
        public string Method { get; set; }

        [JsonPropertyName("Path")]
        public string Path { get; set; }
    }


}
