using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Orleans.Streams;

namespace Grains
{
    public class User : Grain, IUser
    {
        private readonly ILogger<User> _logger;
        private readonly IGrainFactory _grainFactory;
        private readonly List<string> _chats = new(); 

        public User(IGrainFactory grainFactory, ILogger<User> logger)
        {
            _logger = logger;
            _grainFactory = grainFactory;
        }
        
        /*public async override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            // is the UserNotifier initialization needed here???
            _grainFactory.GetGrain<IUserNotifier>(this.GetPrimaryKeyString());
            await Task.CompletedTask;
        }
*/
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

        public async Task SendMessage(string chatRoom, string message)
        {
            if (_chats.Contains(chatRoom))
            {
                var streamProvider = this.GetStreamProvider("chat");
                var chatStream = streamProvider.GetStream<string>(StreamId.Create("ROOM", chatRoom));
                await chatStream.OnNextAsync(message);
                await Task.CompletedTask;
            }
            await Task.FromException(new ArgumentException("User is not allowed to send message to this chat or chat does not exist"));
        }

        public async Task<List<string>> GetChats()
        {
            return await Task.FromResult(_chats);
        }

        public async Task JoinChatRoom(string chatRoomId)
        {
            if (_chats.Contains(chatRoomId))
            {
                _logger.LogWarning($"Chatroom {chatRoomId} is already in {this.GetPrimaryKeyString()}'s chats list");
            }
            else
            {
                _chats.Add(chatRoomId);
                _logger.LogInformation($"Chatroom {chatRoomId} has been added to {this.GetPrimaryKeyString()}'s chats list");
            }

            await Task.CompletedTask;
        }
    }
}
