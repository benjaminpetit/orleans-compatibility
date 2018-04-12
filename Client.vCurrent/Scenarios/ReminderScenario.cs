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
    public class ReminderScenario : MarshalByRefObject, IReminderScenario
    {
        private IClusterClient client;
        private IReminderTestGrain grain;

        public ReminderScenario(IClusterClient client)
        {
            this.client = client;
        }

        public async Task Setup(TimeSpan period)
        {
            this.grain = this.client.GetGrain<IReminderTestGrain>(Guid.NewGuid());
            await grain.SetReminder(period);
        }

        public async Task Teardown()
        {
            await this.grain.RemoveReminder();
            this.grain = null;
        }

        public async Task WaitDifferentRuntimeIdentity()
        {
            var value = await this.grain.GetLastTickValue();

            while (value == await grain.GetLastTickValue())
            {
                await Task.Delay(100);
            }
        }
    }
}
