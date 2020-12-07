using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace BookShop.Tests.Framework
{
    public class ScenarioTestCase : XunitTestCase
    {
        private static readonly ConcurrentDictionary<string, int> ErrorsPerScenario =
            new ConcurrentDictionary<string, int>();

        [Obsolete(
            "Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public ScenarioTestCase()
        {
        }

        public ScenarioTestCase(
            IMessageSink diagnosticMessageSink,
            TestMethodDisplay defaultMethodDisplay,
            TestMethodDisplayOptions displayOptions,
            ITestMethod testMethod,
            object[] testMethodArguments = null)
            : base(diagnosticMessageSink, defaultMethodDisplay, displayOptions, testMethod, testMethodArguments)
        {
        }

        public string ScenarioId => TestMethod.TestClass.Class.Name;

        public override async Task<RunSummary> RunAsync(
            IMessageSink diagnosticMessageSink,
            IMessageBus messageBus,
            object[] constructorArguments,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
        {
            var errorCount = ErrorsPerScenario.GetOrAdd(ScenarioId, 0);
            if (errorCount > 0)
            {
                SkipReason = "Skipped as there are errors in previous steps.";
            }

            var summary = await base.RunAsync(
                diagnosticMessageSink,
                messageBus,
                constructorArguments,
                aggregator,
                cancellationTokenSource);
            ErrorsPerScenario.AddOrUpdate(ScenarioId, summary.Failed, (key, value) => value + summary.Failed);
            return summary;
        }
    }
}