using ETL.Application.Common.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ETL.Infrastructure.Security;

public static class AuthorizationExtension
{
    public static IServiceCollection AddPolicyAuthorization(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policy.CanCreateUser, policy =>
                policy.RequireRole(Role.SystemAdmin));

            options.AddPolicy(Policy.CanReadAllUsers, policy =>
                policy.RequireRole(Role.SystemAdmin));

            options.AddPolicy(Policy.CanChangeUserRole, policy =>
                policy.RequireRole(Role.SystemAdmin));

            options.AddPolicy(Policy.CanDeleteUser, policy =>
                policy.RequireRole(Role.SystemAdmin));

            options.AddPolicy(Policy.CanReadUser, policy =>
                policy.RequireRole(Role.SystemAdmin));

            options.AddPolicy(Policy.CanReadRoles, policy =>
                policy.RequireRole(Role.SystemAdmin));

            options.AddPolicy(Policy.CanUploadFile, policy =>
                policy.RequireRole(Role.SystemAdmin, Role.DataAdmin));

            options.AddPolicy(Policy.CanReadDataSets, policy =>
                policy.RequireRole(Role.SystemAdmin, Role.DataAdmin, Role.Analyst));

            options.AddPolicy(Policy.CanManipulateDataSets, policy =>
                policy.RequireRole(Role.SystemAdmin, Role.DataAdmin));

            options.AddPolicy(Policy.CanViewWorkflows, policy =>
                policy.RequireRole(Role.SystemAdmin, Role.DataAdmin, Role.Analyst));

            options.AddPolicy(Policy.CanManageWorkflows, policy =>
                policy.RequireRole(Role.SystemAdmin, Role.DataAdmin, Role.Analyst));

        });

        return services;
    }
}