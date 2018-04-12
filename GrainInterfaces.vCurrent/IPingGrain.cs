using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces.vCurrent
{
    public interface IPingGrain : IGrainWithGuidKey
    {
        Task<int> Ping();

        Task<string> GetRuntimeIdentity();
    }
}
