

namespace GrainInterfaces
{
    public interface IUserNotifier : IGrainWithStringKey, IGrainObserver
    {
        Task ReceiveNotification(string notification);
        Task<List<string>> RetriveNotifications();

        Task TrySaveNotifier(string ownerusername);

        Task<string> GetOwnerUsername();
    }
}
