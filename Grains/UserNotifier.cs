

using GrainInterfaces;

namespace Grains
{
    public class UserNotifier : Grain, IUserNotifier
    {

        private readonly List<string> _notifications = new();

        public Task ReceiveNotification(string notification)
        {
            _notifications.Add(notification);
            return Task.CompletedTask;
        }

        public Task<List<string>> RetriveNotifications()
        {
            return Task.FromResult(_notifications);
        }
    }
}
