using Xunit;
using Xunit.Sdk;

namespace BookShop.Tests.Framework
{
    [XunitTestCaseDiscoverer("BookShop.Tests.Framework.ScenarioTestCaseDiscoverer", "BookShop.Tests")]
    public class StepAttribute : FactAttribute
    {
    }
}