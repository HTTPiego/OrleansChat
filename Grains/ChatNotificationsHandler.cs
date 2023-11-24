using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans.Utilities;

namespace Grains
{
    public class ChatNotificationsHandler : Grain, IChatNotificationsHandler
    {
        private readonly ObserverManager<IChatsManager> _chatsManagers;

        //TODO: handle timer 
        public ChatNotificationsHandler(ILogger<IChatsManager> logger)
        {
            _chatsManagers = new ObserverManager<IChatsManager>(TimeSpan.FromMinutes(5), logger);
        }

        public Task<ObserverManager<IChatsManager>> GetChatsManagers()
        {
            return Task.FromResult(_chatsManagers);
        }

        public Task Subscribe(IChatsManager chatsManager)
        {
            // userNotifier == null
            if (_chatsManagers.Contains(chatsManager))
            {
                return Task.CompletedTask; //exception
            }
            _chatsManagers.Subscribe(chatsManager, chatsManager);
            return Task.CompletedTask;
        }

        public Task Unsubscribe(IChatsManager chatsManager)
        {
            // userNotifier == null
            if (!_chatsManagers.Contains(chatsManager))
            {
                return Task.CompletedTask; //exception
            }
            _chatsManagers.Subscribe(chatsManager, chatsManager);
            return Task.CompletedTask;
        }

        /*public Task HandleRemovalFrom(IGroupChatRoom groupChat, string notification, IUser whoRemoved)
        {
            _chatsManagers.Notify(chatManager => chatManager.updateGroupsAfterRemoval(groupChat, notification),
                                    chatManager => !chatManager.Equals(whoRemoved.GetChatsManager().Result));
            return Task.FromResult(groupChat);
        }*/

        public Task HandleNotificationFrom(IGroupChatRoom groupChat, string notification, IUser messageAuthor)
        {
            _chatsManagers.Notify(chatManager => chatManager.ReceiveNotificationFrom(groupChat, notification),
                                    chatManager => !chatManager.Equals(messageAuthor.GetChatsManager().Result));
            return Task.FromResult(groupChat);
        }

        public Task HandleNotificationFrom(IDirectChatRoom directChat, string notification, IUser messageAuthor)
        {
            _chatsManagers.Notify(chatManager => chatManager.ReceiveNotificationFrom(directChat, notification),
                                    chatManager => !chatManager.Equals(messageAuthor.GetChatsManager().Result));
            return Task.FromResult(directChat);
        }

    }
}
