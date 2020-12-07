using System.Collections.Generic;
using BookShop.Tests.Framework;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Respawn;

namespace BookShop.Tests.Fixtures
{
    public class BookShopApiFactory : WebApplicationFactory<Startup>
    {
        private readonly SqlServerDatabaseDependency _sqlServerDatabaseDependency;

        public BookShopApiFactory()
        {
            _sqlServerDatabaseDependency = new SqlServerDatabaseDependency();
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(c =>
            {
                c.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["ConnectionStrings:Database"] = _sqlServerDatabaseDependency
                        .SqlServerDatabaseConnectionString
                });
            });
            builder.ConfigureServices(services =>
            {
                // Non-application dependencies
                services.AddSingleton<Checkpoint>();
                services.AddSingleton(_ => new BookShopApiClient(CreateDefaultClient()));
                services.AddSingleton(_sqlServerDatabaseDependency);
                services.AddSingleton<IScenarioDependency>(sp => sp.GetRequiredService<SqlServerDatabaseDependency>());
                services.AddScoped<IScenarioDependency, SqlDatabaseSchemaDependency>();
                services.AddSingleton<ScenarioDependencyRegistry>();
            });
            return base.CreateHost(builder);
        }
    }
}