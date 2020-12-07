using System.Threading.Tasks;

namespace BookShop.Tests.Framework
{
    public interface IScenarioStepDependency
    {
        Task ExecuteAfterScenarioStepAsync();
    }
}