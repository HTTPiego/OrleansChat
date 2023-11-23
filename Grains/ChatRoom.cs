using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans.Utilities;
using System;


namespace Grains
{
    public class ChatRoom : Grain, IChatRoom
    {
        private readonly ObserverManager<IUser> _observers;
        private List<IUser> _users = new List<IUser>();
        private List<string> _messages = new List<string>();

        //TODO: handle timer 
        public ChatRoom(ILogger<IChatRoom> logger)
        {
            _observers = new ObserverManager<IUser>(TimeSpan.FromMinutes(5), logger);
        }

        /*public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            return base.OnActivateAsync(cancellationToken);
        }*/


        private Task Subscribe(IUser user)
        {
            if (user == null || _observers.Contains(user))
            {
                return Task.CompletedTask; //exception
            }
            _observers.Subscribe(user, user);
            return Task.CompletedTask;
        }

        private Task Unsubscribe(IUser user)
        {
            if (user == null || !_observers.Contains(user))
            {
                return Task.CompletedTask; //exception
            }
            _observers.Unsubscribe(user);
            return Task.CompletedTask;
        }

        public Task PostMessage(string message, IUser messageAuthor)
        {
            if (message == String.Empty)
            {
                return Task.CompletedTask; //exception
            }
            _messages.Add(message);
            _observers.Notify(observer =>
                                observer.ReceiveNotificationFrom("Message from " + messageAuthor.GetUserNickname(), this),
                                observer => !observer.GetPrimaryKey().Equals(messageAuthor.GetPrimaryKey()));
            return Task.CompletedTask;
        }

        public Task<List<string>> getMessages()
        {
            return Task.FromResult(_messages);
        }

        public Task addUser(IUser whoAdds, IUser user)
        {
            if (user == null || _users.Contains(user))
            {
                return Task.CompletedTask; //exception  
            }
            _users.Add(user);
            Subscribe(user);
            _observers.Notify(observer => observer.ReceiveNotificationFrom("You have been added to new chat!", this),
                                            observer => !whoAdds.Equals(observer));
            return Task.CompletedTask;
        }

        public Task removeUser(IUser whoAdds, IUser user)
        {
            if (user == null || !_users.Contains(user))
            {
                return Task.CompletedTask; //exception
            }
            _users.Remove(user);
            Unsubscribe(user);
            //TODO: handle user permission 
            _observers.Notify(observer => observer.ReceiveNotificationFrom("You have been added to new chat!", this),
                                            observer => !whoAdds.Equals(observer));
            return Task.CompletedTask;
        }
    }
}
