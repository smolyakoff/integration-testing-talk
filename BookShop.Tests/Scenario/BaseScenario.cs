using BookShop.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BookShop.Tests.Scenario
{
    public abstract class BaseScenario : Framework.Scenario, IClassFixture<BookShopFixture>
    {
        private readonly BookShopFixture _fixture;

        protected BaseScenario(BookShopFixture fixture)
        {
            _fixture = fixture;
        }

        protected BookShopApiClient Client => _fixture.Services.GetRequiredService<BookShopApiClient>();
    }
}