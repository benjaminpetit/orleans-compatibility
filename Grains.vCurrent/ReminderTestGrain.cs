using GrainInterfaces.vCurrent;
using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grains
{
    public class ReminderTestGrain : Grain, IRemindable, IReminderTestGrain
    {
        private string lastTickValue;

        private const string ReminderName = "TestReminder";

        public Task<string> GetLastTickValue()
        {
            return Task.FromResult(this.lastTickValue);
        }

        public async Task RemoveReminder()
        {
            await this.UnregisterReminder(await this.GetReminder(ReminderName));
        }

        public async Task SetReminder(TimeSpan period)
        {
            await this.RegisterOrUpdateReminder(ReminderName, TimeSpan.Zero, period);
        }

        public Task ReceiveReminder(string reminderName, TickStatus status)
        {
            this.lastTickValue = this.RuntimeIdentity;
            return Task.CompletedTask;
        }
    }
}
