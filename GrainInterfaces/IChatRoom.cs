using Orleans.Streams;

namespace GrainInterfaces
{
    public struct MessageWithAuthor
    {
        public Guid author;
        public string message;
    }

    public interface IChatRoom : IGrainWithStringKey, IAsyncObserver<MessageWithAuthor>
    {
        Task<List<string>> GetMessages();
        Task<List<string>> GetMembers();
        Task AddUser(string newMember);
        Task Leave(string member);
        Task AddMultipleUsers(List<string> newMembers);
    }
}
