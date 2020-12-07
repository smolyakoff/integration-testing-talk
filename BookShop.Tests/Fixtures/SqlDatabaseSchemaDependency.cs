using System.Threading.Tasks;
using BookShop.Api;
using BookShop.Application.Infrastructure;
using BookShop.Application.Infrastructure.Database;
using BookShop.Tests.Framework;

namespace BookShop.Tests.Fixtures
{
    internal class SqlDatabaseSchemaDependency : IScenarioDependency
    {
        private readonly BookShopDbContext _dbContext;

        public SqlDatabaseSchemaDependency(BookShopDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task ExecuteBeforeScenarioAsync()
        {
            await _dbContext.Database.EnsureCreatedAsync();
        }

        public Task ExecuteAfterScenarioAsync()
        {
            return Task.CompletedTask;
        }
    }
}