using Grains.DTOs;
using GrainInterfaces;
using Grains.GrainState;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Orleans.Runtime;
using Orleans.Streams;

namespace Grains
{
    public class User : Grain, IUser
    {
        private readonly ILogger<User> _logger;
        private readonly IGrainFactory _grainFactory;
        private readonly IPersistentState<UserState> _userState;

        public User(
            [PersistentState("state")] IPersistentState<UserState> userState,
            IGrainFactory grainFactory, ILogger<User> logger)
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

            return await GetUserState();
        }
        
        public Task<List<string>> ReadNotifications()
        {
            var notifier = _grainFactory.GetGrain<IUserNotifier>(this.GetPrimaryKeyString());
            return notifier.RetriveNotifications();
        }

        public async Task SendMessage(string chatRoom, string message)
        {
            if (_userState.State.Chats.Contains(chatRoom))
            {
                var streamProvider = this.GetStreamProvider("chat");
                var chatStream = streamProvider.GetStream<string>(StreamId.Create("ROOM", chatRoom));
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

        public async Task<UserDTO> GetUserState()
        {
            return await Task.FromResult(new UserDTO(
                    name: _userState.State.Name,
                    username: _userState.State.Username,
                    chats: _userState.State.Chats,
                    friends: _userState.State.Friends
                ));
        }
        
        public async Task<UserPersonalDataDTO> GetUserPersonalState()
        {
            return await Task.FromResult(new UserPersonalDataDTO(_userState.State.Name, _userState.State.Username));
        }

    }
}
