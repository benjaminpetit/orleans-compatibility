using System.Threading.Tasks;
using Tests.Common;

namespace Tests.Scenarios
{
    public interface IPingScenario : ITestScenario
    {
        Task PingGrains(int numberOfGrains);
    }
}