using GrainInterfaces.vCurrent;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Scenarios;
using Xunit;

namespace Client.Scenarios
{
    public class PingScenario : MarshalByRefObject, IPingScenario
    {
        private IClusterClient client;

        public PingScenario(IClusterClient client)
        {
            this.client = client;
        }

        public async Task PingGrains(int numberOfGrains)
        {
            var grains = await GetGrainOnDifferentSilos(numberOfGrains);
            foreach (var grain in grains)
            {
                for (int i = 0; i < 10; i++)
                {
                    Assert.Equal(i + 1, await grain.Ping());
                }
            }
        }

        private async Task<IList<IPingGrain>> GetGrainOnDifferentSilos(int numberOfGrains)
        {
            var results = new List<IPingGrain>(numberOfGrains);
            var knownIdentities = new HashSet<string>();

            for (int i=0; i<numberOfGrains; i++)
            {
                IPingGrain grain;
                do
                {
                    grain = this.client.GetGrain<IPingGrain>(Guid.NewGuid());
                }
                while (!knownIdentities.Add(await grain.GetRuntimeIdentity()));
                results.Add(grain);
            }

            return results;
        }
    }
}
