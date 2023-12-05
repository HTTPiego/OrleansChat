using GrainInterfaces;
using Orleans.Runtime;
using Orleans.Streams;

namespace Grains
{
    public class User : Grain, IUser
    {
        private readonly IGrainFactory _grainFactory;
        private readonly List<Guid> _chats = new(); 
        // or Dictionary<Guid, StreamId> _chatsWithTheirStreams ?  THIS does not need to store the Stream Ids, just the ChatRooms GUIDS cause they are the same
        // 
        //private readonly Guid userNotifier; --> same of the owner user
        //private readonly Dictionary<Guid, Guid> _chatAndSubscriptionHandle = new();

        public User(IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }

        public async override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            // is the UserNotifier initialization needed here???
            _grainFactory.GetGrain<IUserNotifier>(this.GetPrimaryKeyString());
            await Task.CompletedTask;
        }

        /*foreach (var pair in _chatAndSubscriptionHandle)
            {
                var chatGuid = pair.Key.ToString();
                var subGuid = pair.Value;
                var streamProvider = this.GetStreamProvider("chat");
                var streamId = StreamId.Create(chatGuid + "_stream", chatGuid); 
                var stream = streamProvider.GetStream<string>(streamId);
                var subscriptionHandles = await stream.GetAllSubscriptionHandles()
                                                        .ContinueWith(allHandles => allHandles.Result.SkipWhile(handle => !handle.HandleId.Equals(subGuid)));
                if (subscriptionHandles != null &&
                    subscriptionHandles.Count() != 0) //!subscriptionHandles.IsNullOrEmpty()
                {
                    foreach (var subscriptionHandle in subscriptionHandles) //subscriptionHandles.ForEach( async x => await x.ResumeAsync(OnNextAsync));
                    {
                        await subscriptionHandle.ResumeAsync(OnNextAsync);
                    }
                }
            }*/

        public Task<List<string>> ReadNotifications()
        {
            var notifier = _grainFactory.GetGrain<IUserNotifier>(this.GetPrimaryKeyString());
            return notifier.RetriveNotifications();
        }

        public async Task SendMessage(Guid chatRoom, string message)
        {
            if (chatRoom in _chats)
            {
                var streamProvider = this.GetStreamProvider("chat");
                var chatStream = streamProvider.GetStream<string>();
                await chatStream.OnNextAsync(message);
                await Task.CompletedTask;
            }
            await Task.FromException(new ArgumentException("User is not allowed to send messsage this chat or chat does not exist"));
        }

        public async Task<List<StreamId>> GetChats()
        {
            return await Task.FromResult(_chats);
        }


    }
}
