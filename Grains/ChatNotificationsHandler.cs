using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans.Utilities;

namespace Grains
{
    public class ChatNotificationsHandler : Grain, IChatNotificationsHandler
    {
        private readonly ObserverManager<IChatsManager> _observers;

        //TODO: handle timer 
        public ChatNotificationsHandler(ILogger<IChatsManager> logger)
        {
            _observers = new ObserverManager<IChatsManager>(TimeSpan.FromMinutes(5), logger);
        }

        public Task<ObserverManager<IChatsManager>> GetChatsManagers()
        {
            return Task.FromResult(_observers);
        }

        public Task Subscribe(IChatsManager chatsManager)
        {
            // userNotifier == null
            if (_observers.Contains(chatsManager))
            {
                return Task.CompletedTask; //exception
            }
            _observers.Subscribe(chatsManager, chatsManager);
            return Task.CompletedTask;
        }

        public Task Unsubscribe(IChatsManager chatsManager)
        {
            // userNotifier == null
            if (!_observers.Contains(chatsManager))
            {
                return Task.CompletedTask; //exception
            }
            _observers.Subscribe(chatsManager, chatsManager);
            return Task.CompletedTask;
        }

        public Task HandleRemovalFrom(IGroupChatRoom groupChat, string notification, IUser whoRemoved)
        {
            _observers.Notify(observer => observer.updateGroupsAfterRemoval(groupChat, notification),
                                observer => !observer.Equals(whoRemoved.GetChatsManager().Result));
            return Task.FromResult(groupChat);
        }

        public Task HandleNotificationFrom(IGroupChatRoom groupChat, string notification, IUser messageAuthor)
        {
            _observers.Notify(observer => observer.ReceiveNotificationFrom(groupChat, notification),
                                observer => !observer.Equals(messageAuthor.GetChatsManager().Result));
            return Task.FromResult(groupChat);
        }

        public Task HandleNotificationFrom(IDirectChatRoom directChat, string notification, IUser messageAuthor)
        {
            _observers.Notify(observer => observer.ReceiveNotificationFrom(directChat, notification),
                                observer => !observer.Equals(messageAuthor.GetChatsManager().Result));
            return Task.FromResult(directChat);
        }

    }
}
