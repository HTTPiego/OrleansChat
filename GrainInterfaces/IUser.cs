using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces
{
    public interface IUser : IGrainWithStringKey, IAsyncObserver<string>
    {
        Task<Dictionary<Guid, Guid>> GetChatAndSubscriptionHandle();
    }
}
