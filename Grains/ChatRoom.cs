using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Orleans.Streams;
using Orleans.Utilities;

namespace Grains
{
    // IF we add the following attribute, the ChatRoom is automatically subscribed to the stream with namespace "ROOM"
    // and GUID is the same as the ChatRoom GUID. So there is no need to save the stream
    
    [ImplicitStreamSubscription("ROOM")] 
    public class ChatRoom : Grain, IChatRoom
    {
        private readonly IGrainFactory _grainFactory;
        private readonly List<Guid> _chatRoomMembers = new();
        private readonly ObserverManager<IUserNotifier> _userNotifiersManager;
        private readonly List<string> _messages = new();


        public ChatRoom(IGrainFactory grainFactory, ILogger<IUserNotifier> logger)
        {
            _grainFactory = grainFactory;
            _userNotifiersManager = new ObserverManager<IUserNotifier>(TimeSpan.FromMinutes(5), logger);
        }

        public override async Task OnActivateAsync(CancellationToken ct)
        {
            if (_messageStreamId.Equals(null))
            {
                var guid = this.GetPrimaryKeyString();
                _messageStreamId = StreamId.Create(guid + "_messageStream", guid); // guid does not need to be appended to the stream namespace
                var messageStream = RetriveStream(_messageStreamId);
                await messageStream.SubscribeAsync(this);
            }
            await base.OnActivateAsync(ct);
        }

        private IAsyncStream<MessageWithAuthor> RetriveStream(StreamId messageStreamId)
        {
            var streamProvider = this.GetStreamProvider("chat");
            return streamProvider.GetStream<MessageWithAuthor>(messageStreamId);
        }

        /*public async Task PostMessage(IUser author, string message)
        {
            if (author == null)
            {
                await Task.FromException(new ArgumentException("Author is invalid"));
            }
            if (! _chatRoomMembers.Contains(author!))
            {
                await Task.FromException(new ArgumentException("This user is not a memeber of this chat"));
            }
            if (String.IsNullOrEmpty(message))
            {
                await Task.FromException(new ArgumentException("Invalid message"));
            }
            _messages.Add(message);
            await _stream.OnNextAsync(author.GetPrimaryKeyString() + " wrote: " + message);

            return;
        }*/

        public async Task<List<string>> GetMessages()
        {
            return await Task.FromResult(_messages);
        }

        public async Task Add(IUser newMember)
        {
            if (newMember == null)
            {
                await Task.FromException(new ArgumentException("New member is invalid"));
            }
            if (_chatRoomMembers.Contains(newMember!.GetPrimaryKey()))
            {
                await Task.FromException(new ArgumentException("This user is already a memeber of this chat"));
            }
            _chatRoomMembers.Add(newMember!.GetPrimaryKey());
            var notification = newMember.GetPrimaryKeyString() + " joined your \"" + this.GetPrimaryKeyString() + "\" chat!";
            await _userNotifiersManager.Notify(notifier => notifier.ReceiveNotification(notification));
            var userNotifier = _grainFactory.GetGrain<IUserNotifier>(newMember.GetPrimaryKeyString());
            _userNotifiersManager.Subscribe(userNotifier, userNotifier);

            await Task.CompletedTask;

            /*
            var sub = await _stream.SubscribeAsync(newMember!);
            newMember!.GetChatAndSubscriptionHandle().Result.Add(this.GetPrimaryKey(), sub.HandleId);
            await _stream.OnNextAsync(notification);
            
            return _stream.StreamId;*/
        }


        public async Task Leave(IUser member)
        {
            if (member == null)
            {
                await Task.FromException(new ArgumentException("The member to be removed is invalid"));
            }
            if (!_chatRoomMembers.Contains(member!.GetPrimaryKey()))
            {
                await Task.FromException(new ArgumentException("This user is not a memeber of this chat"));
            }
            _chatRoomMembers.Remove(member!.GetPrimaryKey());
            var userNotifier = _grainFactory.GetGrain<IUserNotifier>(member.GetPrimaryKeyString());
            _userNotifiersManager.Unsubscribe(userNotifier);
            var notification = member.GetPrimaryKeyString() + " leaved your \"" + this.GetPrimaryKeyString() + "\" chat!";
            await _userNotifiersManager.Notify(notifier => notifier.ReceiveNotification(notification));

            await Task.CompletedTask;

            /*string notification = member.GetPrimaryKeyString() + " leaved your \"" + this.GetPrimaryKeyString() + "\" chat!";
            await _stream.OnNextAsync(notification);
            await Unsubscrive(member!);

            return _stream.StreamId;*/
        }

        /*private async Task Unsubscrive(IUser member)
        {
            var chatAndSubscriptionHandle = await member.GetChatAndSubscriptionHandle();
            foreach (var pair in chatAndSubscriptionHandle)
            {
                var subGuid = pair.Value;
                var subscriptionHandles = await _stream.GetAllSubscriptionHandles()
                                                        .ContinueWith(allHandles => allHandles.Result.SkipWhile(handle => !handle.HandleId.Equals(subGuid)));
                if (subscriptionHandles != null &&
                    subscriptionHandles.Count() != 0)
                {
                    foreach (var subscriptionHandle in subscriptionHandles)
                    {
                        await subscriptionHandle.UnsubscribeAsync();
                    }
                }
            }
            return;
        }*/


        public Task OnCompletedAsync()
        {
            return Task.CompletedTask;
        }

        public Task OnErrorAsync(Exception ex)
        {
            return Task.CompletedTask;
        }

        public Task OnNextAsync(MessageWithAuthor item, StreamSequenceToken? token = null)
        {
            _messages.Add(item.message);
            var notification = "New message!";
            _userNotifiersManager.Notify(notifier => notifier.ReceiveNotification(notification),
                                            notifier => ! notifier.GetPrimaryKey().Equals(item.author)); //if user notifier is not that one of the author
            return Task.CompletedTask;
        }
    }
}
