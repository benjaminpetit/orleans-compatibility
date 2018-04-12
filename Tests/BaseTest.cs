using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public abstract class BaseTest : IDisposable
    {
        protected TestCluster testCluster = new TestCluster();

        public void Dispose()
        {
            this.testCluster.Dispose();
        }
    }
}
