using System.Threading.Tasks;

namespace BookShop.Tests.Framework
{
    public interface IScenarioDependency
    {
        Task ExecuteBeforeScenarioAsync();
        Task ExecuteAfterScenarioAsync();
    }
}