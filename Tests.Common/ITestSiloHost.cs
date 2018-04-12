using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Common
{
    public interface ITestSiloHost
    {
        void Start();

        void Shutdown();
    }
}
