using Grains.DTOs;
using Grains.GrainState;
using Orleans.Streams;

namespace GrainInterfaces
{
    public struct MessageWithAuthor
    {
        public Guid author;
        public string message;
    }

    public interface IChatRoom : IAsyncObserver<MessageWithAuthor>, IGrainWithStringKey //IGrainWithGuidKey
    {
        Task<List<string>> GetMessages();
        Task<List<string>> GetMembers();
        Task AddUser(string newMember);
        Task Leave(string member);
        Task AddMultipleUsers(List<string> newMembers);
        Task<ChatRoomState> GetChatState();
        Task<ChatRoomDTO> TrySaveChat(string chatName);
    }
}
