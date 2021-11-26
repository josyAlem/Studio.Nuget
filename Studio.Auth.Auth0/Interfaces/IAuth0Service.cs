using Studio.Auth.Auth0.Models;
using System.Threading.Tasks;

namespace Studio.Auth.Auth0.Interfaces
{
    public   interface IAuth0Service
    {
        Task<Auth0LoginResponse> Login(Auth0LoginRequest loginRequest);
        Task<Auth0SignupResponse> SignUp(Auth0SignupRequest signupRequest);
        Task<Auth0RefreshTokenResponse> Refresh(Auth0RefreshTokenRequest refreshTokenRequest);
        Task<Auth0UserInfoResponse> GetUserInfo(Auth0UserInfoRequest auth0UserInfoRequest);
    }
}
