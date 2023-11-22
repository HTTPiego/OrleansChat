namespace GrainInterfaces
{
    public interface IUser : IGrainObserver, IGrainWithGuidKey
    {
        Task<string> GetUserNickname();
        Task ReceiveNotificationFrom(string notification, IChatRoom chat);

        Task<List<string>> readMessages(IChatRoom chat);

        Task<IChatRoom> InitializeChat(IUser whoStartedTheChat, IUser friend, IGrainFactory grainFactory);
        Task<IChatRoom> CreateGroupChat(IUser groupCreator, List<IUser> members, IGrainFactory grainFactory);

        Task LeaveGroupChat(IChatRoom chat);

    }
}
