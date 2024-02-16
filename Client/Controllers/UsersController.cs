using System.Collections.Concurrent;
using Grains.DTOs;
using Client.Contracts;
using GrainInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Client.Repositories.Interfaces;
using Grains;
using Client.SignalR;
using Microsoft.AspNetCore.SignalR;
using SignalR.Orleans.Core;

namespace Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        //TODO:
        private readonly IUserRepository _userRepository;

        private readonly IGrainFactory _grainFactory;

        private ILogger<UsersController> _logger;

        private readonly IHubContext<ChatHub> _hubContext;


        public UsersController(IUserRepository userRepository, 
                                IGrainFactory grainFactory, 
                                ILogger<UsersController> logger,
                                IHubContext<ChatHub> hubContext)
        {
            _userRepository = userRepository;
            _grainFactory = grainFactory;
            _logger = logger;
            _hubContext = hubContext;
        }

        //TODO refactor both GetAllUsers and SearchUsersByUsername, only difference is match pattern
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsers();

            List<UserPersonalDataDTO> usersResponse = new ();

            foreach(var user in users)
            {
                var userGrain = _grainFactory.GetGrain<IUser>(user.Username);
                await userGrain.GetUserPersonalStateDTO().ContinueWith(dto => usersResponse.Add(dto.Result));
            }

            return Ok(usersResponse);
        }

        /*var users = await _userRepository.GetAllUsers();

        ConcurrentBag<UserPersonalDataDTO> usersResponse = new ConcurrentBag<UserPersonalDataDTO>();

        await Parallel.ForEachAsync(users, async (user, ct) =>
            {
            var userGrain = _grainFactory.GetGrain<IUser>(user.Username);
            usersResponse.Add(await userGrain.GetUserState().Result.State.GetUserPersonalStateDTO());
        });

            return Ok(usersResponse);*/

        [HttpGet("users-by")]
        public async Task<IActionResult> SearchUsersBySubstring([FromQuery(Name = "search")] string search)
        {
            var users = await _userRepository.GetAllUsersBySubstring(search);

            List<UserPersonalDataDTO> usersResponse = new();

            foreach (var user in users)
            {
                var userGrain = _grainFactory.GetGrain<IUser>(user.Username);
                await userGrain.GetUserPersonalStateDTO().ContinueWith(dto => usersResponse.Add(dto.Result));
            }

            return Ok(usersResponse);
        }


        /*ConcurrentBag<UserPersonalDataDTO> usersResponse = new ConcurrentBag<UserPersonalDataDTO>();

        await Parallel.ForEachAsync(users, async (user, ct) =>
            {
            var userGrain = _grainFactory.GetGrain<IUser>(user.Username);
            //usersResponse.Add(await userGrain.GetUserState().Result.State.GetUserPersonalStateDTO());
        });*/


        [HttpGet("master-user")]
        public async Task<IActionResult> GetOrCreateMasterUser()
        {
            var newUser = _grainFactory.GetGrain<IUser>("orleans.master");
            // Persists user's data if it does not exist already
            var master = await newUser.TryCreateUserRetDTO("Orleans Master", "orleans.master");
            try
            {
                await newUser.ObtainUserDB().ContinueWith(userdb => _userRepository.AddUser(userdb.Result));
                //_userRepository.AddUser(newUser.GetUserState().Result.State.ObtainUserDB());
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }

            return Ok(master);
        }


        [HttpPost("new")]
        public async Task<IActionResult> CreateUser(NewUserRequest request)
        {
            if (String.IsNullOrEmpty(request.Username))
            {
                return BadRequest();
            }

            var newUser = _grainFactory.GetGrain<IUser>(request.Username);
            // Persists user's data if it does not exist already
            await newUser.TryCreateUserRetDTO(request.Name, request.Username);

            try
            {
               await newUser.ObtainUserDB().ContinueWith(userdb => _userRepository.AddUser(userdb.Result));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }

            return Ok(request.Username);
        }

        [HttpGet("chats-preview-by")]
        public async Task<IActionResult> RetriveChatsPreviewsBy([FromQuery(Name = "username")] string username)
        {
            if (String.IsNullOrEmpty(username))
            {
                throw new ArgumentException("...");
            }
            if (_userRepository.UserIsRegistered(username) == null)
            {
                throw new ArgumentException("...");
            }

            var userGrain = _grainFactory.GetGrain<IUser>(username);
            
            var response = new List<ChatPreviewDTO>(); //here there is the chat in which the mess has been written

            var chats = userGrain.GetUserChats().Result;

            foreach (var chat in chats)
            {
                var chatGrain = _grainFactory.GetGrain<IChatRoom>(chat);

                await chatGrain.GetChatRoomPreview().ContinueWith(chatDTO => response.Add(chatDTO.Result));
            }

            return Ok(response);
        }

        [HttpGet("{username}/friends")]
        public async Task<IActionResult> RetriveUserFriends(string username)
        {
            if (String.IsNullOrEmpty(username))
            {
                throw new ArgumentException("...");
            }
            if (_userRepository.UserIsRegistered(username) == null)
            {
                throw new ArgumentException("...");
            }

            var userGrain = _grainFactory.GetGrain<IUser>(username);


            return Ok(userGrain.GetUserFriends().Result);
        }

        [HttpPost("send-message")]      
        public async Task SendMessage([FromBody]UserMessage message)
         {
            if (message.TextMessage.IsNullOrEmpty())
            {
                throw new Exception("...");
            }
            var userGrain = _grainFactory.GetGrain<IUser>(message.AuthorUsername);
            var chat = _grainFactory.GetGrain<IChatRoom>(message.ChatRoomName);
            await chat.GetChatname().ContinueWith(name => Console.WriteLine(name.Result));
            await userGrain.SendMessage(message);

            await _hubContext.Clients.All.SendAsync("chatRoomUpdate", message);
            //await _hubContext.Clients.All.ReveiceMessage(message);
            //await _hubContext.Clients.Group(message.ChatRoomName).ReveiceMessage(message);
        }


    }
}
