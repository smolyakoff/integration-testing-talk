using System.Threading.Tasks;
using BookShop.Application.Infrastructure;
using BookShop.Application.Infrastructure.Database;
using BookShop.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Xunit;

namespace BookShop.Tests.Classic
{
    public abstract class BaseApiTest : IClassFixture<BookShopFixture>, IAsyncLifetime
    {
        private readonly BookShopFixture _fixture;
        private IServiceScope _testMethodScope;

        protected BaseApiTest(BookShopFixture fixture)
        {
            _fixture = fixture;
        }

        protected BookShopApiClient Client => _fixture.Services.GetRequiredService<BookShopApiClient>();

        internal BookShopDbContext DbContext =>
            _testMethodScope.ServiceProvider.GetRequiredService<BookShopDbContext>(); 

        public async Task InitializeAsync()
        {
            _testMethodScope = _fixture.Services.CreateScope();
            var dockerEnvironment = _fixture.Services.GetRequiredService<SqlServerDatabaseDependency>();
            var checkpoint = _fixture.Services.GetRequiredService<Checkpoint>();
            await checkpoint.Reset(dockerEnvironment.SqlServerDatabaseConnectionString);
        }

        public Task DisposeAsync()
        {
            _testMethodScope.Dispose();
            return Task.CompletedTask;
        }
    }
}