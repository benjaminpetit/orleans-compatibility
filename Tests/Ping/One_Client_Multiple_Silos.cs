using System;
using System.Threading.Tasks;
using Tests.Scenarios;
using Xunit;

namespace Tests
{
    public class One_Client_Multiple_Silos : BaseTest
    {
        [Fact]
        public void VCurrentClient_VNextSilo()
        {
            TestImpl(Versions.Current);
        }

        [Fact]
        public void VNextClient_VCurrentSilo()
        {
            TestImpl(Versions.Next);
        }

        private void TestImpl(string clientVersion)
        {
            var silo1 = this.testCluster.StartSilo(Versions.Current);
            var silo2 = this.testCluster.StartSilo(Versions.Next);
            var client = this.testCluster.StartClient(clientVersion);

            client.Execute<IPingScenario>(async scenario =>
            {
                await scenario.PingGrains(2);
            });
        }
    }
}
