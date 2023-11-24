namespace GrainInterfaces
{
    public interface IUser : IGrainWithGuidKey//, IGrainObserver
    {
        Task SetUserNickname(string nickname);
        Task<string> GetUserNickname();
        Task<IChatsManager> GetChatsManager();

    }
}
