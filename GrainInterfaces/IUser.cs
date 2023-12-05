using Orleans.Runtime;

namespace GrainInterfaces
{
    public interface IUser : IGrainWithStringKey
    {
        Task SendMessage(IChatRoom chatRoom, string message);
        Task<List<StreamId>> GetChats();
    }
}
