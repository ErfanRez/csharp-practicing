using ETL.Application.Behaviors;
using ETL.Application.Common.Options;
using ETL.Application.User.GetById;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ETL.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration config)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        services.AddValidatorsFromAssemblyContaining<GetUserByIdQueryValidator>();

        services.Configure<AuthOptions>(config.GetSection(AuthOptions.SectionName));
        services.Configure<OAuthAdminOptions>(config.GetSection(OAuthAdminOptions.SectionName));
        return services;
    }
}
