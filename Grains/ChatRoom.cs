using ChatSilo.DTOs;
using GrainInterfaces;
using Grains.DTOs;
using Grains.GrainState;
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
        //TODO: USE GUID AS KEY!!!!

        private readonly ILogger<ChatRoom> _logger;
        private readonly IGrainFactory _grainFactory;
        private readonly ObserverManager<IUserNotifier> _userNotifiersManager;
        private readonly IPersistentState<ChatRoomState> _chatroomState;
        /*private readonly List<string> _chatRoomMembers = new();
        private readonly List<string> _messages = new();
        private bool _isGroup = false;*/

        public ChatRoom([PersistentState("state")] IPersistentState<ChatRoomState> chatroomState,
            IGrainFactory grainFactory, 
            ILogger<IUserNotifier> logger, 
            ILogger<ChatRoom> chatroomLogger)
        {
            _logger = chatroomLogger;
            _grainFactory = grainFactory;
            _userNotifiersManager = new ObserverManager<IUserNotifier>(TimeSpan.FromMinutes(5), logger);
            _chatroomState = chatroomState;
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await _chatroomState.ReadStateAsync();
            await base.OnActivateAsync(cancellationToken);
        }

        public async Task<List<string>> GetMembers()
        {
            return await Task.FromResult(_chatroomState.State.ChatRoomMembers);
        }
        public async Task<List<UserMessage>> GetMessages()
        {
            return await Task.FromResult(_chatroomState.State.Messages);
        }

        public async Task AddUser(string newMember)
        {
            if (String.IsNullOrEmpty(newMember))
            {
                await Task.FromException(new ArgumentException("New member is invalid"));
            }
            if (_chatroomState.State.ChatRoomMembers.Contains(newMember))
            {
                _logger.LogWarning($"User {newMember} is already a member of this chat");
            }

            // Changes ChatRoom from Direct to Group
            if (_chatroomState.State.ChatRoomMembers.Count() == 2)
            {
                _chatroomState.State.IsGroup = true;
                _logger.LogInformation($"Chatroom {this.GetPrimaryKeyString()} has become a Group chat");
            }

            _chatroomState.State.ChatRoomMembers.Add(newMember);
            await _chatroomState.WriteStateAsync();
            _logger.LogInformation($"User {newMember} has joined the ChatRoom {this.GetPrimaryKeyString()}");
            var notification = newMember + " joined your \"" + this.GetPrimaryKeyString() + "\" chat!";
            await _userNotifiersManager.Notify(notifier => notifier.ReceiveNotification(notification));
            var userNotifier = _grainFactory.GetGrain<IUserNotifier>(newMember);
            _userNotifiersManager.Subscribe(userNotifier, userNotifier);

            await Task.CompletedTask;
        }
        
        public async Task AddMultipleUsers(List<string> newMembers)
        {
            foreach (var member in newMembers)
            {
                try
                {
                    await AddUser(member);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            await Task.CompletedTask;
        }


        public async Task Leave(string member)
        {
            if (String.IsNullOrEmpty(member))
            {
                await Task.FromException(new ArgumentException("The member to be removed is invalid"));
            }
            if (!_chatroomState.State.ChatRoomMembers.Contains(member))
            {
                await Task.FromException(new ArgumentException("This user is not a memeber of this chat"));
            }
            _chatroomState.State.ChatRoomMembers.Remove(member);
            await _chatroomState.WriteStateAsync();
            var userNotifier = _grainFactory.GetGrain<IUserNotifier>(member);
            _userNotifiersManager.Unsubscribe(userNotifier);
            var notification = member + " leaved your \"" + this.GetPrimaryKeyString() + "\" chat!";
            await _userNotifiersManager.Notify(notifier => notifier.ReceiveNotification(notification));

            await Task.CompletedTask;

        }


        public async Task<ChatRoomDTO> TrySaveChat(string chatName)
        {
            if (String.IsNullOrEmpty(_chatroomState.State.ChatName))
            {
                _chatroomState.State.ChatName = chatName;

                await _chatroomState.WriteStateAsync();

                _logger.LogInformation($"{chatName}'s data has been persisted.");
            }
            else
            {
                _logger.LogWarning($"{chatName} already exists.");
            }

            return await _chatroomState.State.GetChatRoomStateDTO();
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

        public async Task OnNextAsync(UserMessageDTO userMessage, StreamSequenceToken? token = null)
        {
            var message = new UserMessage(userMessage.AuthorUsername, userMessage.ChatRoomName, userMessage.TextMessage, userMessage.Timestamp);
            _chatroomState.State.Messages.Add(message);
            await _chatroomState.WriteStateAsync();
            var notification = "New message!";
            await _userNotifiersManager.Notify(notifier => notifier.ReceiveNotification(notification),
                                            notifier => ! notifier.GetPrimaryKey().Equals(message.AuthorUsername)); //if user notifier is not that one of the author
            await Task.CompletedTask;
        }

        public async Task<ChatRoomState> GetChatState()
        {
            return await Task.FromResult(_chatroomState.State);
        }

    }
}
