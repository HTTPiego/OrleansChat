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
        Task Add(IUser newMember);
        Task Leave(IUser member);
    }
}
