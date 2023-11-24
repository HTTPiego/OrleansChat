using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans.Utilities;
using System;


namespace Grains
{
    public class DirectChatRoom : Grain, IDirectChatRoom
    {
        private readonly ObserverManager<IChatNotificationsHandler> _chatNotificationsHandler;
        //private IChatNotificationsHandler _chatNotificationsHandler;
        private List<IUser> _users = new List<IUser>();
        private List<string> _messages = new List<string>();

        //TODO: handle timer 
        public DirectChatRoom(ILogger<IChatNotificationsHandler> logger)
        {
            _chatNotificationsHandler = new ObserverManager<IChatNotificationsHandler>(TimeSpan.FromMinutes(5), logger);
        }

        public override Task OnActivateAsync(CancellationToken ct)
        {
            var chatNotificationsHandler = GrainFactory.GetGrain<IChatNotificationsHandler>(Guid.NewGuid());
            _chatNotificationsHandler.Subscribe(chatNotificationsHandler, chatNotificationsHandler);
            return base.OnActivateAsync(ct);
        }

        public Task PostMessage(string message, IUser messageAuthor)
        {
            if (message == String.Empty)
            {
                return Task.CompletedTask; //exception
            }
            _messages.Add(message);
            _chatNotificationsHandler.Notify(chatNotificationsHandler => 
                                                chatNotificationsHandler.HandleNotificationFrom(this, "Message from " + messageAuthor.GetUserNickname(), messageAuthor));
            return Task.CompletedTask;
        }

        /*public Task PostMessage(string message, IUser messageAuthor)
        {
            if (message == String.Empty)
            {
                return Task.CompletedTask; //exception
            }
            _messages.Add(message);
            _chatNotificationsHandler.Notify(chatNotificationsHandler => chatNotificationsHandler.HandleNotificationFrom(this, "Message from " + messageAuthor.GetUserNickname()),
                                                chatNotificationsHandler => !chatNotificationsHandler.GetChatsManagers().Result
                                                                            .SkipWhile(mng => mng.Equals(messageAuthor.GetChatsManager().Result))
                                                                            .Contains(messageAuthor.GetChatsManager().Result));
                                //observer => !observer.GetPrimaryKey().Equals(messageAuthor.GetPrimaryKey()));
            return Task.CompletedTask;
        }*/

        public Task<List<string>> getMessages()
        {
            return Task.FromResult(_messages);
        }

        public async Task addUser(IUser whoAdds, IUser user)
        {
            if (user == null || _users.Contains(user))
            {
                return; //exception  
            }
            _users.Add(user);
            await _chatNotificationsHandler.First().Subscribe(user.GetChatsManager().Result);
            await _chatNotificationsHandler.Notify(observer => observer.HandleNotificationFrom(this, "New message from " + whoAdds.GetUserNickname()),
                                                    observer => !whoAdds.Equals(user));
            return;
        }

    }
}
