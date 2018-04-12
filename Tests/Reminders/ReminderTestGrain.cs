using System;
using System.Threading.Tasks;
using Tests.Scenarios;
using Xunit;

namespace Tests
{
    public class ReminderTests : BaseTest
    {
        [Fact]
        public void VCurrentSilo_VNextSilo()
        {
            var silo1 = this.testCluster.StartSilo(Versions.Current);
            var silo2 = this.testCluster.StartSilo(Versions.Next);
            var client = this.testCluster.StartClient(Versions.Current);

            client.Execute<IReminderScenario>(async scenario =>
            {
                await scenario.Setup(TimeSpan.FromMinutes(1));
                await scenario.WaitDifferentRuntimeIdentity();
                await scenario.Teardown();
            });
        }
    }
}
