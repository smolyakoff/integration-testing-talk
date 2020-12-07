using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BookShop.Tests.Framework
{
    public class ScenarioDependencyRegistry
    {
        private readonly List<IScenarioDependency> _scenarioDependencies = new List<IScenarioDependency>();
        private readonly IServiceScope _serviceScope;
        private readonly ILogger<ScenarioDependencyRegistry> _logger;

        public ScenarioDependencyRegistry(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScope = serviceScopeFactory.CreateScope();
            _logger = _serviceScope.ServiceProvider.GetRequiredService<ILogger<ScenarioDependencyRegistry>>();
        }

        public async Task InitializeScenarioAsync()
        {
            var dependencies = _serviceScope.ServiceProvider.GetServices<IScenarioDependency>();
            foreach (var dependency in dependencies)
            {
                _logger.LogInformation("Dependency {DependencyName} is initializing.", dependency.GetType().Name);
                await dependency.ExecuteBeforeScenarioAsync();
                _scenarioDependencies.Add(dependency);
                _logger.LogInformation("Dependency {DependencyName} is initialized.", dependency.GetType().Name);
            }
        }

        public async Task DisposeScenarioAsync()
        {
            async Task SafeDisposeAsync(IScenarioDependency dependency)
            {
                try
                {
                    await dependency.ExecuteAfterScenarioAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, 
                        "Dependency {DependencyName} failed to dispose nicely.",
                        dependency.GetType().Name);
                }
            }
            var reverseDependencies = _scenarioDependencies.ToList();
            reverseDependencies.Reverse();
            foreach (var dependency in reverseDependencies)
            {
                _logger.LogInformation("Dependency {DependencyName} is disposing.", dependency.GetType().Name);
                await SafeDisposeAsync(dependency);
                _logger.LogInformation("Dependency {DependencyName} is disposed.", dependency.GetType().Name);
            }
        }
    }
}