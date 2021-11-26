using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Studio.Auth.Auth0.Models;
using RestSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Studio.Auth.Auth0.Interfaces;

namespace Studio.Auth.Auth0.Services
{
    public class Auth0Service : IAuth0Service
    {
        private readonly RestClient _client;
        private readonly Auth0Settings _auth0Settings;
        public Auth0Service(IOptions<Auth0Settings> auth0Config)
        {
            _auth0Settings = auth0Config.Value;
            _client = new RestClient(_auth0Settings.Domain);
        }
        public async Task<Auth0LoginResponse> Login(Auth0LoginRequest loginRequest)
        {
            loginRequest.Audience = _auth0Settings.AuthAPI.Audience;
            loginRequest.GrantType = _auth0Settings.GrantTypes.PasswordRealm;
            loginRequest.Realm = _auth0Settings.AuthAPI.ConnectionRealm;
            loginRequest.Scope = _auth0Settings.AuthAPI.Scope;
            
            var req = new RestRequest(_auth0Settings.Paths.Token, Method.POST);
            req.AddParameter("client_id", loginRequest.ClientId, ParameterType.GetOrPost);
            req.AddParameter("grant_type", loginRequest.GrantType, ParameterType.GetOrPost);
            req.AddParameter("audience", loginRequest.Audience, ParameterType.GetOrPost);
            req.AddParameter("scope", loginRequest.Scope, ParameterType.GetOrPost);
            req.AddParameter("username", loginRequest.UserName, ParameterType.GetOrPost);
            req.AddParameter("password", loginRequest.Password, ParameterType.GetOrPost);
            req.AddParameter("realm", loginRequest.Realm, ParameterType.GetOrPost);
            IRestResponse response = await _client.ExecuteAsync(req);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Log.Error("Login:Access Token cannot be obtained! " + response.Content);
                throw new Exception(response.Content);

            }

            var loginResponse = JsonConvert.DeserializeObject<Auth0LoginResponse>(response.Content);

            var auth0UserinfoResponse = await GetUserInfo(new Auth0UserInfoRequest { AccessToken = loginResponse.AccessToken, Audience = _auth0Settings.AuthAPI.Audience });

            loginResponse.IsAdmin = auth0UserinfoResponse.Roles.Any(c=>c.ToLower() == _auth0Settings.AuthAPI.AdminRole.ToLower());
            loginResponse.Permissions = auth0UserinfoResponse.Permissions;
            loginResponse.Roles = auth0UserinfoResponse.Roles;

            return loginResponse;
        }

        public async Task<Auth0RefreshTokenResponse> Refresh(Auth0RefreshTokenRequest refreshTokenRequest)
        {
            refreshTokenRequest.GrantType = _auth0Settings.GrantTypes.Refresh;
            var req = new RestRequest(_auth0Settings.Paths.Token, Method.POST);
            req.AddParameter("client_id", refreshTokenRequest.ClientId, ParameterType.GetOrPost);
            req.AddParameter("grant_type", refreshTokenRequest.GrantType, ParameterType.GetOrPost);
            req.AddParameter("refresh_token", refreshTokenRequest.RefreshToken, ParameterType.GetOrPost);
            IRestResponse response = await _client.ExecuteAsync(req);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Log.Error("Refresh:Access Token cannot be obtained");
                throw new Exception(response.Content);

            }

            var refreshResponse = JsonConvert.DeserializeObject<Auth0RefreshTokenResponse>(response.Content);
            return refreshResponse;
        }

        public async Task<Auth0SignupResponse> SignUp(Auth0SignupRequest signupRequest)
        {
            signupRequest.Connection = _auth0Settings.AuthAPI.ConnectionRealm;
            var getTokenResponse = await GetManagementAPIToken(_auth0Settings.Domain + _auth0Settings.ManagementAPI.Token.Path,
                _auth0Settings.AuthAPI.ClientId, _auth0Settings.AuthAPI.ClientSecret, _auth0Settings.GrantTypes.Client);

            var req = new RestRequest(_auth0Settings.ManagementAPI.Token.Path 
                + _auth0Settings.ManagementAPI.Users.Path, Method.POST);
            req.AddHeader("authorization", "Bearer " + getTokenResponse.AccessToken);
            req.AddHeader("content-type", "application/json");
            req.RequestFormat = DataFormat.Json;
            req.AddJsonBody(new
            {
                email = signupRequest.Email,
                password = signupRequest.Password,
                connection = signupRequest.Connection
            });
            IRestResponse response = await _client.ExecuteAsync(req);

            if (response.StatusCode != HttpStatusCode.Created)
            {
                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    Log.Error("User already exists");
                    throw new Exception("User already exists");
                }
                else
                {
                    Log.Error("Access Token cannot be obtained, process terminate");
                    throw new Exception(response.Content);
                }
            }
            var signupResponse = JsonConvert.DeserializeObject<Auth0SignupResponse>(response.Content);
            return signupResponse;
        }

        private async Task<List<Auth0RoleResponse>> GetUserRoles(string userId)
        {
            var getTokenResponse = await GetManagementAPIToken(_auth0Settings.Domain + _auth0Settings.ManagementAPI.Token.Path,
                _auth0Settings.AuthAPI.ClientId, _auth0Settings.AuthAPI.ClientSecret, _auth0Settings.GrantTypes.Client);

            var req = new RestRequest(_auth0Settings.ManagementAPI.Token.Path 
                + _auth0Settings.ManagementAPI.Users.Path + "/" + userId + "/" + _auth0Settings.ManagementAPI.Roles.Path, Method.GET);
            req.AddHeader("authorization", "Bearer " + getTokenResponse.AccessToken);
            IRestResponse response = await _client.ExecuteAsync(req);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Log.Error("Access Token cannot be obtained, process terminate");
                throw new Exception(response.Content);

            }
            var roleResponse = JsonConvert.DeserializeObject<List<Auth0RoleResponse>>(response.Content);
            return roleResponse;
        }


        private async Task<List<Auth0PermissionResponse>> GetUserPermissions(string userId)
        {
            var getTokenResponse = await GetManagementAPIToken(_auth0Settings.Domain + _auth0Settings.ManagementAPI.Token.Path,
              _auth0Settings.AuthAPI.ClientId, _auth0Settings.AuthAPI.ClientSecret, _auth0Settings.GrantTypes.Client);

            var req = new RestRequest(_auth0Settings.ManagementAPI.Token.Path 
                + _auth0Settings.ManagementAPI.Users.Path + "/" + userId + "/" 
                + _auth0Settings.ManagementAPI.Permissions.Path, Method.GET);

            req.AddHeader("authorization", "Bearer " + getTokenResponse.AccessToken);
            IRestResponse response = await _client.ExecuteAsync(req);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Log.Error("Access Token cannot be obtained, process terminate");
                throw new Exception(response.Content);

            }
            var permissionsResponse = JsonConvert.DeserializeObject<List<Auth0PermissionResponse>>(response.Content);
            return permissionsResponse;
        }
        public async Task<Auth0UserInfoResponse> GetUserInfo(Auth0UserInfoRequest auth0UserInfoRequest)
        {

            var req = new RestRequest(_auth0Settings.Paths.UserInfo, Method.POST);
            req.AddHeader("authorization", "Bearer " + auth0UserInfoRequest.AccessToken);
            IRestResponse response = await _client.ExecuteAsync(req);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Log.Error("User info cannot be obtained");
                throw new Exception(response.Content);
            }

            var tokenResponse = JsonConvert.DeserializeObject<Auth0UserInfoResponse>(response.Content);
            var auth0UserPermissionResponse = await GetUserPermissions(tokenResponse.UserId);
            var auth0UserRoleResponse = await GetUserRoles(tokenResponse.UserId);
            tokenResponse.Permissions = auth0UserPermissionResponse.Where(c=>c.SourceApp==auth0UserInfoRequest.Audience).Select(c=>c.Name).ToList();
            tokenResponse.Roles = auth0UserRoleResponse.Select(c=>c.Name).ToList();
            return tokenResponse;
        }
        private async Task<Auth0TokenResponse> GetManagementAPIToken(string audience, string clientId, string clientSecret, string grantType)
        {

            var req = new RestRequest(_auth0Settings.Paths.Token, Method.POST);
            req.AddParameter("client_id", clientId, ParameterType.GetOrPost);
            req.AddParameter("grant_type", grantType, ParameterType.GetOrPost);
            req.AddParameter("client_secret", clientSecret, ParameterType.GetOrPost);
            req.AddParameter("audience", audience, ParameterType.GetOrPost);
            IRestResponse response = await _client.ExecuteAsync(req);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Log.Error("Access Token cannot be obtained");
                throw new Exception(response.Content);
            }

            var tokenResponse = JsonConvert.DeserializeObject<Auth0TokenResponse>(response.Content);
            return tokenResponse;
        }

    }
}
