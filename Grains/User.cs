using Grains.DTOs;
using GrainInterfaces;
using Grains.GrainState;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;

namespace Grains
{
    public class User : Grain, IUser
    {
        //TODO: USE GUID AS KEY!!!!

        private readonly ILogger<User> _logger;
        private readonly IGrainFactory _grainFactory;
        private readonly IPersistentState<UserState> _userState;

        public User(
            [PersistentState("state")] IPersistentState<UserState> userState,
            IGrainFactory grainFactory, 
            ILogger<User> logger)
        {
            _logger = logger;
            _grainFactory = grainFactory;
            _userState = userState;
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await _userState.ReadStateAsync();
            await base.OnActivateAsync(cancellationToken);
        }
        
        public async Task<UserDTO> TryCreateUser(string name, string username)
        {
            if (String.IsNullOrEmpty(_userState.State.Username))
            {
                
                _userState.State.Username = username;
                _userState.State.Name = name;

                await _userState.WriteStateAsync();

                _logger.LogInformation($"{username}'s data has been persisted.");
            }
            else
            {
                _logger.LogWarning($"{username} already exists.");
            }

            return await _userState.State.GetUserStateDTO();
        }
        
        public Task<List<string>> ReadNotifications()
        {
            var notifier = _grainFactory.GetGrain<IUserNotifier>(this.GetPrimaryKeyString());
            return notifier.RetriveNotifications();
        }

        public async Task SendMessage(UserMessage message)
        {
            var chatname = message.ChatRoomName;
            if (_userState.State.Chats.Contains(chatname))
            {
                var streamProvider = this.GetStreamProvider("chat");
                var chatStream = streamProvider.GetStream<UserMessage>(StreamId.Create("ROOM", chatname));
                await chatStream.OnNextAsync(message);
                await Task.CompletedTask;
            }
            await Task.FromException(new ArgumentException("User is not allowed to send message to this chat or chat does not exist"));
        }


        public async Task JoinChatRoom(string chatRoomId)
        {
            if (_userState.State.Chats.Contains(chatRoomId))
            {
                _logger.LogWarning($"Chatroom {chatRoomId} is already in {this.GetPrimaryKeyString()}'s chats list");
            }
            else
            {
                _userState.State.Chats.Add(chatRoomId);
                _logger.LogInformation($"Chatroom {chatRoomId} has been added to {this.GetPrimaryKeyString()}'s chats list");
            }

            await Task.CompletedTask;
        }

        public async Task AddFriend(string username)
        {
            if (_userState.State.Friends.Contains(username))
            {
                _logger.LogWarning($"{this.GetPrimaryKeyString()} already has {username} in their friends list");
            }
            else
            {
                _userState.State.Friends.Add(username);
                await _userState.WriteStateAsync();
                _logger.LogInformation($"{username} has been added to {this.GetPrimaryKeyString()}'s friends list");
            }

            await Task.CompletedTask;
            
        }

        public async Task<UserState> GetUserState()
        {
            return await Task.FromResult( _userState.State );
        }

        /*public async Task<UserDTO> GetUserStateDTO()
        {
            return await _userState.State.GetUserStateDTO();
        }
        
        public async Task<UserPersonalDataDTO> GetUserPersonalStateDTO()
        {
            return await _userState.State.GetUserPersonalStateDTO();
        }*/

    }
}
