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
        Task PostMessage(IUser author, string message);
        Task<List<string>> GetMessages();
        Task<StreamId> Add(IUser newMember);
        Task<StreamId> Leave(IUser member);
    }
}
