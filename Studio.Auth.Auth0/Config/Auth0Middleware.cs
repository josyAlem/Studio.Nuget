using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Studio.Auth.Auth0.Models;
using Microsoft.Extensions.Configuration;
using Studio.Auth.Auth0.Interfaces;
using Studio.Auth.Auth0.Services;

namespace Studio.Auth.Auth0.Config
{
    public static class Auth0Middleware
    {
        public static void Init(IServiceCollection services, IConfiguration auth0Config, List<Type> scopeTypes)
        {
            var auth0Setting = new Auth0Settings();
            auth0Config.Bind(auth0Setting);
            services.Configure<Auth0Settings>(auth0Config);

            string domain = auth0Setting.Domain;
            string audience = auth0Setting.AuthAPI.Audience;
            AddAuthentication(services, domain, audience);
            AddAuthorization(services, scopeTypes, domain);

            // Register the scope authorization handler
            services.AddSingleton<IAuthorizationHandler, AuthScopeHandler>();
            services.AddScoped<IAuth0Service, Auth0Service>();
        }
        private static void AddAuthentication(IServiceCollection services, string domain, string audience)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.Authority = domain;
                        options.Audience = audience;

                    });
        }
        private static void AddAuthorization(IServiceCollection services, List<Type> scopeTypes, string domain)
        {
            services.AddAuthorization(options =>
            {
                foreach (Type st in scopeTypes)
                {
                    List<FieldInfo> scopeProps = GetConstants(st);

                    scopeProps.ForEach(t =>
                    {
                        string scopeValue = t.GetRawConstantValue().ToString();

                        options.AddPolicy(scopeValue, policy => policy.Requirements.Add(new AuthScopeRequirement(scopeValue, domain)));
                    });
                }
            });
        }
        private static List<FieldInfo> GetConstants(Type type)
        {
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public |
                 BindingFlags.Static | BindingFlags.FlattenHierarchy);

            return fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
        }


    }
}
