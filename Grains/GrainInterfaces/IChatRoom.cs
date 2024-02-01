using ChatSilo.DTOs;
using Grains.DTOs;
using Grains.GrainState;
using Orleans.Streams;

namespace GrainInterfaces
{
    public struct UserMessage
    {
        public string AuthorUsername { get; set; }
        public string ChatRoomName { get; set; }
        public string TextMessage { get; set; }
        public DateTime Timestamp { get; set; }

        public UserMessage(string author, string chat, string message, DateTime date) 
        {
            AuthorUsername = author;
            ChatRoomName = chat;
            TextMessage = message;
            Timestamp = date;
        }    
    }

    public interface IChatRoom : IAsyncObserver<UserMessageDTO>, IGrainWithStringKey //IGrainWithGuidKey
    {
        Task<List<UserMessage>> GetMessages();
        Task<List<string>> GetMembers();
        Task AddUser(string newMember);
        Task Leave(string member);
        Task AddMultipleUsers(List<string> newMembers);
        Task<ChatRoomState> GetChatState();
        Task<ChatRoomDTO> TrySaveChat(string chatName);
    }
}
