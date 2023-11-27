using Orleans.Utilities;

namespace GrainInterfaces

{
    public interface IChatNotificationsHandler : IGrainObserver, IGrainWithGuidKey
    {
        Task<ObserverManager<IChatsManager>> GetChatsManagers();
        Task Subscribe(IChatsManager chatsManager);
        Task Unsubscribe(IChatsManager chatsManager);
        //Task HandleRemovalFrom(IGroupChatRoom groupChat, string notification, IUser whoRemoved);
        Task HandleNotificationFrom(IDirectChatRoom directChat, string notification, IUser messageAuthor);
        Task HandleNotificationFrom(IGroupChatRoom groupChat, string notification, IUser messageAuthor);
    }
}
