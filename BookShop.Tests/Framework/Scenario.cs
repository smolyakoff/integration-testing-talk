using Xunit;

namespace BookShop.Tests.Framework
{
    [TestCaseOrderer("BookShop.Tests.Framework.ScenarioOrderer", "BookShop.Tests")]
    public abstract class Scenario
    {
    }
}