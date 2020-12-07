using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace BookShop.Tests.Framework
{
    public class ScenarioOrderer : ITestCaseOrderer
    {
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
            where TTestCase : ITestCase
        {
            return testCases.OrderBy(x => x.TestMethod.Method.Name);
        }
    }
}