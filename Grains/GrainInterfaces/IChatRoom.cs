using ChatSilo.DTOs;
using Grains;
using Grains.DTOs;
using Grains.GrainState;
using Orleans.Streams;

namespace GrainInterfaces
{
    [GenerateSerializer]
    public class UserMessage
    {
        [Id(0)]
        public string AuthorUsername { get; set; }
        [Id(1)]
        public string ChatRoomName { get; set; }
        [Id(2)]
        public string TextMessage { get; set; }
        [Id(3)]
        public DateTime Timestamp { get; set; }

        /*public UserMessage(string author, string chat, string message, DateTime date) 
        {
            AuthorUsername = author;
            ChatRoomName = chat;
            TextMessage = message;
            Timestamp = date;
        } */   
    }

    public interface IChatRoom : IAsyncObserver<UserMessage>, IGrainWithStringKey //IGrainWithGuidKey
    {
        Task<List<UserMessage>> GetMessages();
        Task<List<string>> GetMembers();
        Task AddUser(string newMember);
        Task Leave(string member);
        Task AddMultipleUsers(List<string> newMembers);
        Task<ChatRoomState> GetChatState();

        Task<ChatRoomDB> ObtainChatRoomDB();
        Task<ChatRoomDTO> TrySaveChat(string chatName);
    }
}
