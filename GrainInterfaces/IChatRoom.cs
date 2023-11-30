using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrainInterfaces
{
    public interface IChatRoom : IGrainWithStringKey
    {
        Task PostMessage(IUser_ author, string message);
        Task<List<string>> GetMessages();
        Task<StreamId> Add(IUser_ newMember);
        Task<StreamId> Leave(IUser_ member);
    }
}
