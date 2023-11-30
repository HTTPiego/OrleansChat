using Orleans.Streams;

namespace GrainInterfaces
{
    public interface IDirectChatRoom : IGrainWithGuidKey//, IAsyncObserver<string>
    {
        Task PostMessage(string message, IUser messageAuthor);
        Task<List<string>> getMessages();
        Task addUser(IUser whoAdds, IUser user);

    }
}