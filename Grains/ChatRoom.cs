using GrainInterfaces;
using Grains.DTOs;
using Grains.GrainState;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Orleans.Streams;
using Orleans.Utilities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Client.SignalR;
using SignalR.Orleans.Core;

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
        private IPersistentState<ChatRoomState> _chatroomState;
        //private readonly HubContext<ChatHub> _hubContext;
        //private readonly ChatHub _chatHub;
        /*private readonly List<string> _chatRoomMembers = new();
        private readonly List<string> _messages = new();
        private bool _isGroup = false;*/

        public ChatRoom([PersistentState("state")] IPersistentState<ChatRoomState> chatroomState,
            IGrainFactory grainFactory, 
            ILogger<IUserNotifier> logger,
            ILogger<ChatRoom> chatroomLogger/*,
            HubContext<ChatHub> hubContext,
            ChatHub chatHub*/)
        {
            _logger = chatroomLogger;
            _grainFactory = grainFactory;
            _userNotifiersManager = new ObserverManager<IUserNotifier>(TimeSpan.FromMinutes(5), logger);
            _chatroomState = chatroomState;
            //_hubContext = hubContext;
            //_chatHub = chatHub;
        }

        /*public async override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            var chatname = _chatroomState.State.ChatName;
            var streamProvider = this.GetStreamProvider("chat");
            var streamId = StreamId.Create("MyStreamNamespace", chatname);
            var stream = streamProvider.GetStream<string>(streamId);

            var subscriptionHandles = await stream.GetAllSubscriptionHandles();
            if (!subscriptionHandles.IsNullOrEmpty())
            {
                subscriptionHandles.ForEach(
                    async x => await x.ResumeAsync(OnNextAsync));
            }
        }*/

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await _chatroomState.ReadStateAsync();

            var chatname = _chatroomState.State.ChatName;

            if (chatname != null)
            {
                Console.WriteLine(chatname);

                var streamProvider = this.GetStreamProvider("chat");

                var streamId = StreamId.Create("ROOM", chatname);
                var stream = streamProvider.GetStream<UserMessage>(streamId);
                await stream.SubscribeAsync(OnNextAsync);
            }

            await base.OnActivateAsync(cancellationToken);

        }

        public async Task<List<string>> GetMembers()
        {
            return await Task.FromResult(_chatroomState.State.ChatRoomMembers);
        }

        public async Task<string> GetChatname()
        {
            return await Task.FromResult(_chatroomState.State.ChatName);
        }

        public async Task<List<UserMessage>> GetMessages()
        {
            await _chatroomState.ReadStateAsync();
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
            await userNotifier.TrySaveNotifier(newMember);
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

        public async Task OnNextAsync(UserMessage message, StreamSequenceToken? token = null)
        {
            _chatroomState.State.Messages.Add(message);
            await _chatroomState.WriteStateAsync();
            _logger.LogCritical("____________________________CIAO @#@#@#@#@#@#@ CIAO ======> CIAO___________________________________");

            /*foreach (var notifier in _userNotifiersManager) 
            {
                var notiferOwner = await notifier.GetOwnerUsername();
                if (!notiferOwner.Equals(message.AuthorUsername))
                {
                    await notifier.ReceiveNotification("New message!");
                }
            }*/

            await _userNotifiersManager.Notify(notifier => notifier.ReceiveNotification("New message!"),
                                                 notifier => !notifier.GetPrimaryKeyString().Equals(message.AuthorUsername));

            //if user notifier is not that one of the author

            /*var msg = new InvocationMessage("ReceiveMessage", new object?[] { message });
            await _hubContext.Group(message.ChatRoomName).Send(msg);*/

            //await _hubContext.Clients.All.SendAsync("ReceiveMessage", message);

            await Task.CompletedTask;
        }

        public async Task<ChatRoomState> GetChatState()
        {
            return await Task.FromResult(_chatroomState.State);
        }

        public Task<ChatRoomDB> ObtainChatRoomDB()
        {
            return Task.FromResult(new ChatRoomDB(_chatroomState.State.ChatName));
        }

        public async Task<ChatRoomDTO> GetChatRoomStateDTO()
        {
            var chatname = _chatroomState.State.ChatName;
            var members = _chatroomState.State.ChatRoomMembers;
            return await Task.FromResult(new ChatRoomDTO(chatname, members));
        }

    }

    [GenerateSerializer]

    public class ChatRoomDB
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Id(0)]
        public Guid ChatRoomId { get; set; }

        [Id(1)]
        public string ChatName { get; set; } = default!;

        public ChatRoomDB() { }

        public ChatRoomDB(string chatRoomName)
        {
            //ChatRoomId = new Guid();
            ChatName = chatRoomName;
        }
    }

}
