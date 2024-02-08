using Grains.DTOs;
using GrainInterfaces;
using Grains.GrainState;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;
using Orleans;
using Orleans.Streams;

namespace Grains
{

    public class User : Grain, IUser
    {
        //TODO: USE GUID AS KEY!!!!

        private readonly ILogger<User> _logger;
        private readonly IGrainFactory _grainFactory;
        private IPersistentState<UserState> _userState;

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
        
        public async Task<UserDTO> TryCreateUserRetDTO(string name, string username)
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

        public async Task<UserState> TryCreateUserRetState(string name, string username)
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

            return _userState.State;
        }

        public Task<List<string>> ReadNotifications()
        {
            var notifier = _grainFactory.GetGrain<IUserNotifier>(this.GetPrimaryKeyString());
            return notifier.RetriveNotifications();
        }

        public async Task SendMessage(UserMessage message)
        {
            var chatname = message.ChatRoomName;
            Console.WriteLine("QUI");
            if (_userState.State.Chats.Contains(chatname))
            {
                Console.WriteLine("QUO");
                //_grainFactory.GetGrain<IChatRoom>(message.ChatRoomName);
                var streamProvider = this.GetStreamProvider("chat");
                //Console.WriteLine("QUA");
                var chat = _grainFactory.GetGrain<IChatRoom>(message.ChatRoomName);
                var chatStream = streamProvider.GetStream<UserMessage>(StreamId.Create("ROOM", chat.GetPrimaryKeyString()));
                Console.WriteLine("QUA");
                await chatStream.OnNextAsync(message);
                Console.WriteLine("BINGO");
            }
            else
            {
                await Task.FromException(new ArgumentException("User is not allowed to send message to this chat or chat does not exist"));
            }
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
                await _userState.WriteStateAsync();
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

        public async Task<UserPersonalDataDTO> GetUserPersonalStateDTO()
        {
            var name = _userState.State.Name;
            var username = _userState.State.Username;
            return await Task.FromResult(new UserPersonalDataDTO(name, username));
        }

        public async Task<IPersistentState<UserState>> GetUserState()
        {
            await _userState.ReadStateAsync();
            return await Task.FromResult(_userState);
        }

        public async Task<List<string>> GetUserFriends()
        {
            return await Task.FromResult(_userState.State.Friends);
        }

        public async Task<List<string>> GetUserChats()
        {
            return await Task.FromResult(_userState.State.Chats);
        }

        public async Task<string> GetUsername()
        {
            await _userState.ReadStateAsync();
            return await Task.FromResult(_userState.State.Username);
        }

        public async Task<UserDB> ObtainUserDB()
        {
            return await Task.FromResult(new UserDB(_userState.State.Username));
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

    [GenerateSerializer]
    public class UserDB
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Id(0)]
        public Guid UserId { get; set; }

        [Id(1)]
        public string Username { get; set; }

        public UserDB() { }

        public UserDB(string username)
        {
            //UserId = new Guid();
            Username = username;
        }

    }


}
