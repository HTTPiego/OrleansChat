using GrainInterfaces;
using Orleans.Runtime;
using Orleans.Streams;

namespace Grains
{
    public class ChatRoom : Grain, IChatRoom
    {
        private readonly List<IUser> _chatRoomMembers = new();
        private readonly List<string> _messages = new();
        private IAsyncStream<string> _stream = null!;

        public override Task OnActivateAsync(CancellationToken ct)
        {
            var streamProvider = this.GetStreamProvider("chat");
            var guid = this.GetPrimaryKeyString();
            var streamId = StreamId.Create(guid + "_stream", guid);
            _stream = streamProvider.GetStream<string>(streamId);

            return base.OnActivateAsync(ct);
        }

        public async Task PostMessage(IUser author, string message)
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
        }

        public async Task<List<string>> GetMessages()
        {
            return await Task.FromResult(_messages);
        }

        public async Task<StreamId> Add(IUser newMember)
        {
            if (newMember == null)
            {
                await Task.FromException(new ArgumentException("New member is invalid"));
            }
            if (_chatRoomMembers.Contains(newMember!))
            {
                await Task.FromException(new ArgumentException("This user is already a memeber of this chat"));
            }
            _chatRoomMembers.Add(newMember!);
            string notification = newMember.GetPrimaryKeyString() + " joined your \"" + this.GetPrimaryKeyString() + "\" chat!";
            var sub = await _stream.SubscribeAsync(newMember!);
            newMember!.GetChatAndSubscriptionHandle().Result.Add(this.GetPrimaryKey(), sub.HandleId);
            await _stream.OnNextAsync(notification);
            
            return _stream.StreamId;
        }


        public async Task<StreamId> Leave(IUser member)
        {
            if (member == null)
            {
                await Task.FromException(new ArgumentException("The member to be removed is invalid"));
            }
            if (!_chatRoomMembers.Contains(member!))
            {
                await Task.FromException(new ArgumentException("This user is not a memeber of this chat"));
            }
            _chatRoomMembers.Remove(member!);
            string notification = member.GetPrimaryKeyString() + " leaved your \"" + this.GetPrimaryKeyString() + "\" chat!";
            await _stream.OnNextAsync(notification);
            await Unsubscrive(member!);

            return _stream.StreamId;
        }

        private async Task Unsubscrive(IUser member)
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
        }

    }
}
