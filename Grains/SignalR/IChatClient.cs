using GrainInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.SignalR
{
    public interface IChatClient
    {
        Task ReveiceMessage(UserMessage message);
    }
}
