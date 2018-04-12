using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tests.Common;

namespace Tests.Scenarios
{
    public interface IReminderScenario : ITestScenario
    {
        Task Setup(TimeSpan period);

        Task WaitDifferentRuntimeIdentity();

        Task Teardown();
    }
}
