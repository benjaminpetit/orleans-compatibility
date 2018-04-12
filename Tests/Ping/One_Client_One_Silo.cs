using System;
using System.Threading.Tasks;
using Tests.Scenarios;
using Xunit;

namespace Tests
{
    public class One_Client_One_Silo : BaseTest
    {
        [Fact]
        public void VCurrentClient_VNextSilo()
        {
            TestImpl(
                siloVersion:    Versions.Next,
                clientVersion:  Versions.Current);
        }

        [Fact]
        public void VNextClient_VCurrentSilo()
        {
            TestImpl(
                siloVersion:    Versions.Current,
                clientVersion:  Versions.Next);
        }

        private void TestImpl(string siloVersion, string clientVersion)
        {
            var silo = this.testCluster.StartSilo(siloVersion);
            var client = this.testCluster.StartClient(clientVersion);

            client.Execute<IPingScenario>(async scenario =>
            {
                await scenario.PingGrains(1);
            });
        }
    }
}
