using GrainInterfaces;
using Orleans.Runtime;
using Orleans.Streams;

namespace Grains
{
    public class ChatRoom : Grain, IChatRoom
    {
        private readonly List<IUser_> _chatRoomMembers = new();
        private readonly List<string> _messages = new();
        private IAsyncStream<string> _stream = null!;
        //private List<StreamSubscriptionHandle<string>> _streamSubscriptionHandleList = null!;

        public override Task OnActivateAsync(CancellationToken ct)
        {
            var streamProvider = this.GetStreamProvider("chat");
            var guid = this.GetPrimaryKeyString();
            var streamId = StreamId.Create(guid + "_stream", guid);
            _stream = streamProvider.GetStream<string>(streamId);

            return base.OnActivateAsync(ct);
        }

        public async Task PostMessage(IUser_ author, string message)
        {
            _messages.Add(message);
            await _stream.OnNextAsync(author.GetPrimaryKeyString() + " wrote: " + message);

            return;
        }

        public async Task<List<string>> GetMessages()
        {
            return await Task.FromResult(_messages);
        }

        public async Task<StreamId> Add(IUser_ newMember)
        {  
            _chatRoomMembers.Add(newMember);
            string notification = newMember.GetPrimaryKeyString() + " joined your \"" + this.GetPrimaryKeyString() + "\" chat!";
            var sub = await _stream.SubscribeAsync(newMember);
            newMember.GetChatAndSubscriptionHandle().Result.Add(this.GetPrimaryKey(), sub.HandleId);
            await _stream.OnNextAsync(notification);
            
            return _stream.StreamId;
        }


        public async Task<StreamId> Leave(IUser_ member)
        {
            _chatRoomMembers.Remove(member);
            string notification = member.GetPrimaryKeyString() + " leaved your \"" + this.GetPrimaryKeyString() + "\" chat!";
            await _stream.OnNextAsync(notification);
            await Unsubscrive(member);

            return _stream.StreamId;
        }

        private async Task Unsubscrive(IUser_ member)
        {
            var chatAndSubscriptionHandle = await member.GetChatAndSubscriptionHandle();
            foreach (var pair in chatAndSubscriptionHandle)
            {
                var subGuid = pair.Value;
                var subscriptionHandles = await _stream.GetAllSubscriptionHandles()
                                                        .ContinueWith(allHandles => allHandles.Result.SkipWhile(handle => !handle.HandleId.Equals(subGuid)));
                if (subscriptionHandles != null &&
                    subscriptionHandles.Count() != 0) //!subscriptionHandles.IsNullOrEmpty()
                {
                    foreach (var subscriptionHandle in subscriptionHandles) //subscriptionHandles.ForEach( async x => await x.ResumeAsync(OnNextAsync));
                    {
                        await subscriptionHandle.UnsubscribeAsync();
                    }
                }
            }
            return;
        }

    }
}
