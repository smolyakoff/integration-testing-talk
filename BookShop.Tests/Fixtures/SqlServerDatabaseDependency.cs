using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BookShop.Tests.Framework;
using TestEnvironment.Docker;
using TestEnvironment.Docker.Containers.Mssql;

namespace BookShop.Tests.Fixtures
{
    internal class SqlServerDatabaseDependency : IScenarioDependency
    {
        private readonly DockerEnvironment _environment;

        public SqlServerDatabaseDependency()
        {
            var sqlServerContainerPort = TcpPortFinder.FindAvailablePort();
            var sqlServerConnectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = $"127.0.0.1,{sqlServerContainerPort}",
                InitialCatalog = "BookShop",
                UserID = "SA",
                Password = "Str0ng!Password"
            };
            SqlServerDatabaseConnectionString = sqlServerConnectionStringBuilder.ToString();
            _environment = new DockerEnvironmentBuilder()
                .AddMssqlContainer(
                    "bookshop-sql", 
                    sqlServerConnectionStringBuilder.Password, 
                    ports: new Dictionary<ushort, ushort>
                    {
                        [1433] = sqlServerContainerPort
                    })
                .Build();
        }

        public string SqlServerDatabaseConnectionString { get; }
        
        public async Task ExecuteBeforeScenarioAsync()
        {
            await _environment.Up();
        }
        
        public async Task ExecuteAfterScenarioAsync()
        {
            await _environment.Down();
        }
    }
}