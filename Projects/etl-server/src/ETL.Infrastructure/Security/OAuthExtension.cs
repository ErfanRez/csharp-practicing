using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ETL.Infrastructure.Security;
public static class OAuthExtension
{
    public static IServiceCollection AddOAuth(this IServiceCollection services, IConfiguration config)
    {
        var section = config.GetSection("Authentication");
        var authority = section["Authority"]!;
        var clientId = section["ClientId"]!;
        const string cookieName = "access_token";

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.Audience = clientId;
                options.RequireHttpsMetadata = false; // Dev only
   
                options.BackchannelHttpHandler = new HttpClientHandler();

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateLifetime = true,
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Cookies.TryGetValue(cookieName, out var token))
                        {
                            context.Token = token;
                        }
                        return Task.CompletedTask;
                    },

                    OnTokenValidated = context =>
                    {
                        if (context.Principal?.Identity is ClaimsIdentity identity &&
                            context.Principal.HasClaim(c => c.Type == "realm_access"))
                        {
                            var realmAccessClaim = context.Principal.FindFirst("realm_access")!.Value;
                            using var realmAccessDoc = JsonDocument.Parse(realmAccessClaim);
                            var realmRoles = realmAccessDoc.RootElement.GetProperty("roles").EnumerateArray();

                            foreach (var role in realmRoles)
                            {
                                identity.AddClaim(new Claim(ClaimTypes.Role, role.GetString()!));
                            }
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }
}
