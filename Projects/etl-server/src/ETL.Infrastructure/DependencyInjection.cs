using ETL.Application.Abstractions.Data;
using ETL.Application.Abstractions.Pipelines;
using ETL.Application.Abstractions.Repositories;
using ETL.Application.Abstractions.Security;
using ETL.Application.Abstractions.UserServices;
using ETL.Infrastructure.Data;
using ETL.Infrastructure.Data.Abstractions;
using ETL.Infrastructure.OAuthClients;
using ETL.Infrastructure.OAuthClients.Abstractions;
using ETL.Infrastructure.Repositories;
using ETL.Infrastructure.Repositories.Abstractions;
using ETL.Infrastructure.Repositories.DataSets;
using ETL.Infrastructure.Repositories.StagingTables;
using ETL.Infrastructure.Security;
using ETL.Infrastructure.Transform.PipelineOperations;
using ETL.Infrastructure.Transform.PluginOperations;
using ETL.Infrastructure.UserServices;
using ETL.Infrastructure.UserServices.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlKata.Compilers;

namespace ETL.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddOAuth(config);
        services.AddPolicyAuthorization(config);
        
        services.AddHttpClient(string.Empty);

        services.AddSingleton<IOAuthGetJson, OAuthGetJsonClient>();
        services.AddSingleton<IOAuthGetJsonArray, OAuthGetJsonArrayClient>();
        services.AddSingleton<IOAuthPostJson, OAuthPostJsonClient>();
        services.AddSingleton<IOAuthPutJson, OAuthPutJsonClient>();
        services.AddSingleton<IOAuthDeleteJson, OAuthDeleteJsonClient>();
        services.AddSingleton<IOAuthPostJsonWithResponse, OAuthPostJsonWithResponseClient>();

        services.AddSingleton<IUserFetcher, UserFetcher>();
        services.AddSingleton<IUserJsonMapper, UserJsonMapper>();
        services.AddSingleton<IUsersRoleFetcher, UsersRoleFetcher>();
        services.AddSingleton<IUserRoleAssigner, UserRoleAssigner>();

        services.AddSingleton<IOAuthUserReader, OAuthUserReader>();
        services.AddSingleton<IOAuthAllUserReader, OAuthAllUserReader>();
        services.AddSingleton<IOAuthUserRoleGetter, OAuthUserRoleGetter>();
        services.AddSingleton<IOAuthUserCreator, OAuthUserCreator>();
        services.AddSingleton<IOAuthUserUpdater, OAuthUserUpdater>();
        services.AddSingleton<IOAuthUserDeleter, OAuthUserDeleter>();
        services.AddSingleton<IOAuthRoleRemover, OAuthRoleRemover>();
        services.AddSingleton<IOAuthRoleAssigner, OAuthRoleAssigner>();
        services.AddSingleton<IAdminTokenService, AdminTokenService>();
        services.AddSingleton<IAuthCodeForTokenExchanger, AuthCodeForTokenExchanger>();
        services.AddSingleton<IAuthTokenRefresher, AuthTokenRefresher>();
        services.AddSingleton<IAuthCredentialValidator, AuthCredentialValidator>();
        services.AddSingleton<IAuthRestPasswordService, AuthRestPasswordService>();
        services.AddSingleton<IAuthLogoutService, AuthLogoutService>();

        services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(config.GetConnectionString("DefaultConnection")));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<Compiler, PostgresCompiler>();

        services.AddSingleton<IGetAllDataSets, GetAllDataSetsOperation>();
        services.AddSingleton<IGetDataSetById, GetDataSetByIdOperation>();
        services.AddSingleton<IGetDataSetByTableName, GetDataSetByTableNameOperation>();
        services.AddSingleton<IAddDataSet, AddDataSetOperation>();
        services.AddSingleton<IUpdateDataSet, UpdateDataSetOperation>();
        services.AddSingleton<IDeleteDataSet, DeleteDataSetOperation>();

        services.AddSingleton<IStreamGetter, StreamGetter>();
        services.AddSingleton<ICsvHeaderReader, CsvHeaderReader>();
        services.AddSingleton<IIdentifierSanitizer, IdentifierSanitizer>();
        services.AddSingleton<ITableCreator, TableCreator>();
        services.AddSingleton<ICsvCopyImporter, CsvCopyImporter>();
        services.AddSingleton<ICreateTableFromCsv, CreateTableFromCsvOperation>();
        services.AddSingleton<IRenameStagingTable, RenameStagingTableOperation>();
        services.AddSingleton<IRenameStagingColumn, RenameStagingColumnOperation>();
        services.AddSingleton<IDeleteStagingTable, DeleteStagingTableOperation>();
        services.AddSingleton<IDeleteStagingColumn, DeleteStagingColumnOperation>();
        services.AddSingleton<IStagingColumnExists, StagingColumnExistsOperation>();
        services.AddSingleton<IGetStagingTableByName, GetStagingTableByName>();
        
        services.AddDbContextFactory<WorkflowDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("DefaultConnection")));

        services.AddSingleton<IGetAllPipelines, GetAllPipelines>();
        services.AddSingleton<IGetPipelineById, GetPipelineById>();
        services.AddSingleton<ICreatePipeline, CreatePipeline>();
        services.AddSingleton<IUpdatePipeline, UpdatePipeline>();
        services.AddSingleton<IDeletePipeline, DeletePipeline>();
        
        services.AddSingleton<IGetPluginById, GetPluginById>();
        services.AddSingleton<IGetPluginsByPipelineId, GetPluginsByPipelineId>();
        services.AddSingleton<IAppendPlugin, AppendPlugin>();
        services.AddSingleton<IUpdatePlugin, UpdatePlugin>();
        services.AddSingleton<IDeletePlugin, DeletePlugin>();
        services.AddSingleton<IGetPipelinesByCreatorId, GetPipelinesByCreatorId>();


        return services;
    }
}
