
namespace GrainInterfaces
{
    public interface IChatsManager : IGrainObserver, IGrainWithGuidKey
    {
        Task<List<string>> getMessages(IDirectChatRoom chat);
        Task<IDirectChatRoom> InitializeDirectChat(IUser chatsManagerOwner, IUser friend, IGrainFactory grainFactory);
        Task<IGroupChatRoom> CreateGroupChat(IUser chatsManagerOwner, List<IUser> members, IGrainFactory grainFactory);
        Task LeaveGroupChat(IUser chatsManagerOwner, IGroupChatRoom chat);
        Task ReceiveNotificationFrom(IDirectChatRoom directChat, string notification);
        Task ReceiveNotificationFrom(IGroupChatRoom groupChat, string notification);
        Task updateGroupsAfterRemoval(IGroupChatRoom groupChat, string notification);
    }
}
