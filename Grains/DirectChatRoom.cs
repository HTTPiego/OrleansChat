using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Orleans.Streams;
using Orleans.Utilities;
using System.Threading;
using System.Xml;


namespace Grains
{
    //[ImplicitStreamSubscription("DIRECT")]
    public class DirectChatRoom : Grain, IDirectChatRoom
    {


        public override Task OnActivateAsync(CancellationToken ct)
        {
            // Pick a GUID for a chat room grain and chat room stream
            //var guid = new Guid("some guid identifying the chat room");
            //_strem.get
            // Get one of the providers which we defined in our config
            var streamProvider = this.GetStreamProvider("Chat");
            // Get the reference to a stream
            var streamId = StreamId.Create("RANDOMDATA", this.GetPrimaryKey());
            var stream = streamProvider.GetStream<int>(streamId);

            return base.OnActivateAsync(ct);
        }





        private readonly ObserverManager<IChatNotificationsHandler> _chatNotificationsHandler;
        private List<IUser> _users = new List<IUser>();
        private List<string> _messages = new List<string>();





        //TODO: handle timer 
        public DirectChatRoom(ILogger<IChatNotificationsHandler> logger)
        {
            _chatNotificationsHandler = new ObserverManager<IChatNotificationsHandler>(TimeSpan.FromMinutes(5), logger);
            //_stream = s;
        }

        /*public override Task OnActivateAsync(CancellationToken ct)
        {
            // Pick a GUID for a chat room grain and chat room stream
            var guid = new Guid("some guid identifying the chat room");
            //_strem.get
            // Get one of the providers which we defined in our config
            var streamProvider = this.GetStreamProvider("StreamProvider");
            // Get the reference to a stream
            var streamId = StreamId.Create("RANDOMDATA", guid);
            var stream = streamProvider.GetStream<int>(streamId);


            var chatNotificationsHandler = GrainFactory.GetGrain<IChatNotificationsHandler>(Guid.NewGuid());
            _chatNotificationsHandler.Subscribe(chatNotificationsHandler, chatNotificationsHandler);
            return base.OnActivateAsync(ct);
        }*/

        public Task PostMessage(string message, IUser messageAuthor)
        {
            if (message == String.Empty)
            {
                return Task.FromException(new ArgumentException());
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
                throw new ArgumentException();
            }
            _users.Add(user);
            await _chatNotificationsHandler.First().Subscribe(user.GetChatsManager().Result);
            await _chatNotificationsHandler.Notify(chatNotificationsHandler => 
                                                    chatNotificationsHandler.HandleNotificationFrom(this, "New message from " + whoAdds.GetUserNickname(), whoAdds));    
            return;
        }

        public Task OnNextAsync(string item, StreamSequenceToken? token = null)
        {
            throw new NotImplementedException();
        }

        public Task OnCompletedAsync()
        {
            throw new NotImplementedException();
        }

        public Task OnErrorAsync(Exception ex)
        {
            throw new NotImplementedException();
        }
    }
}
