using GrainInterfaces;
using Orleans.Runtime;
using Orleans.Streams;

namespace Grains
{
    public class User : Grain, IUser
    {
        private readonly Dictionary<Guid, Guid> _chatAndSubscriptionHandle = new();
        private readonly List<string> _notifications = new();

        public async override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            foreach (var pair in _chatAndSubscriptionHandle)
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
            }
        }

        public async Task<Dictionary<Guid, Guid>> GetChatAndSubscriptionHandle()
        {
            return await Task.FromResult(_chatAndSubscriptionHandle);
        }

        public Task OnCompletedAsync()
        {
            return Task.CompletedTask;
        }

        public Task OnErrorAsync(Exception ex)
        {
            return Task.CompletedTask;
        }

        public Task OnNextAsync(string notification, StreamSequenceToken? token = null)
        {
            _notifications.Add(notification);
            return Task.CompletedTask;
        }


    }
}
