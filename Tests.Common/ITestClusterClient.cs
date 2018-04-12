using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Common
{
    public partial interface ITestClusterClient
    {
        void Connect();

        void Execute<T>(Func<T, Task> func) where T : ITestScenario;
    }
}
