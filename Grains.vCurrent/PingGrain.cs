using GrainInterfaces.vCurrent;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Grains
{
    public class PingGrain : Grain, IPingGrain
    {
        private int counter = 0;

        public Task<string> GetRuntimeIdentity()
        {
            return Task.FromResult(RuntimeIdentity);
        }

        public Task<int> Ping()
        {
            return Task.FromResult(++this.counter);
        }
    }
}
