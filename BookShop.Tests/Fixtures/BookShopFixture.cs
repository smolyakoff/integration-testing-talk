using System;
using System.Threading.Tasks;
using BookShop.Tests.Framework;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BookShop.Tests.Fixtures
{
    public class BookShopFixture : IAsyncLifetime
    {
        private readonly BookShopApiFactory _apiFactory;

        public BookShopFixture()
        {
            _apiFactory = new BookShopApiFactory();
        }

        public IServiceProvider Services => _apiFactory.Services;

        async Task IAsyncLifetime.InitializeAsync()
        {
            var dependencyRegistry = _apiFactory.Services.GetRequiredService<ScenarioDependencyRegistry>();
            await dependencyRegistry.InitializeScenarioAsync();
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            var dependencyRegistry = _apiFactory.Services.GetRequiredService<ScenarioDependencyRegistry>();
            try
            {
                await dependencyRegistry.DisposeScenarioAsync();
            }
            finally
            {
                _apiFactory.Dispose();
            }
        }
    }
}