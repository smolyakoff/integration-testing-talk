using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace BookShop.Tests.Framework
{
    public class ScenarioTestCaseDiscoverer : IXunitTestCaseDiscoverer
    {
        private readonly IMessageSink _messageSink;

        public ScenarioTestCaseDiscoverer(IMessageSink messageSink)
        {
            _messageSink = messageSink;
        }

        public IEnumerable<IXunitTestCase> Discover(
            ITestFrameworkDiscoveryOptions discoveryOptions,
            ITestMethod testMethod,
            IAttributeInfo factAttribute)
        {
            yield return new ScenarioTestCase(
                _messageSink,
                discoveryOptions.MethodDisplayOrDefault(),
                discoveryOptions.MethodDisplayOptionsOrDefault(),
                testMethod);
        }
    }
}