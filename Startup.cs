using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(dotnet_function_sqldb.Startup))]

namespace dotnet_function_sqldb
{
    class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            switch (Environment.GetEnvironmentVariable("UseDatabase").ToLower())
            {
                case "cosmos":
                    builder.Services.AddDbContext<SQLDbContext>(options => options.UseCosmos(
                            Environment.GetEnvironmentVariable("CosmosDbEndpoint"), 
                            Environment.GetEnvironmentVariable("CosmosDbKey"),
                            databaseName: Environment.GetEnvironmentVariable("CosmosDbDatabaseId")));
                    break;
                case "postgresql":
                    builder.Services.AddDbContext<SQLDbContext>(options => options.UseNpgsql(Environment.GetEnvironmentVariable("PostgreSqlConnectionString")));
                    break;
                case "sqlserver":
                default: 
                    builder.Services.AddDbContext<SQLDbContext>(options => options.UseSqlServer(Environment.GetEnvironmentVariable("SqlConnectionString")));
                    break;
            }
        }
    }
}