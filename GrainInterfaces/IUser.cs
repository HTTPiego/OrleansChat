namespace GrainInterfaces
{
    public interface IUser : IGrainWithGuidKey//, IGrainObserver
    {
        Task SetUserNickname(string nickname);
        Task<string> GetUserNickname();
        Task<IChatsManager> GetChatsManager();

        //Task<IUserNotifier> GetUserNotifier();
        Task ReceiveNotification(IChatNotificationsHandler userNotifier, string notification);

        //ask<List<string>> readMessages(IChatRoom chat);

        

    }
}
