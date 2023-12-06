using Orleans.Runtime;

namespace GrainInterfaces
{
    public interface IUser : IGrainWithStringKey
    {
        Task JoinChatRoom(string chatRoomId);
        Task SendMessage(string chatRoom, string message);
        Task<List<string>> GetChats();
    }
}
