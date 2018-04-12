using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces.vCurrent
{
    public interface IReminderTestGrain : IGrainWithGuidKey
    {
        Task SetReminder(TimeSpan period);

        Task RemoveReminder();

        Task<string> GetLastTickValue();
    }
}
