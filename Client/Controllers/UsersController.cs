using System.Collections.Concurrent;
using Grains.DTOs;
using Client.Contracts;
using GrainInterfaces;
using Microsoft.AspNetCore.Mvc;
using Client.Repositories;
using Microsoft.Extensions.Logging;

namespace Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        //TODO:
        private readonly UserRepository _userRepository;

        private readonly IGrainFactory _grainFactory;

        private ILogger<UsersController> _logger;


        public UsersController(UserRepository userRepository, IGrainFactory grainFactory, ILogger<UsersController> logger)
        {
            _userRepository = userRepository;
            _grainFactory = grainFactory;
            _logger = logger;
        }
        
        //TODO refactor both GetAllUsers and SearchUsersByUsername, only difference is match pattern
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsers();

            ConcurrentBag<UserPersonalDataDTO> usersResponse = new ConcurrentBag<UserPersonalDataDTO>();

            await Parallel.ForEachAsync(users, async (userState, ct) =>
            {
                usersResponse.Add(await userState.GetUserPersonalStateDTO());
            });

            return Ok(usersResponse);
        }

        [HttpGet()]
        public async Task<IActionResult> SearchUsersBySubstring([FromQuery(Name = "search")] string search)
        {
            var users = await _userRepository.GetAllUsersBySubstring(search);

            ConcurrentBag<UserPersonalDataDTO> usersResponse = new ConcurrentBag<UserPersonalDataDTO>();

            await Parallel.ForEachAsync(users, async (userState, ct) =>
            {
                usersResponse.Add(await userState.GetUserPersonalStateDTO());
            });

            return Ok(usersResponse);

            
        }

        

        [HttpGet("master-user")]
        public async Task<IActionResult> GetOrCreateMasterUser()
        {
            var newUser = _grainFactory.GetGrain<IUser>("orleans.master");
            // Persists user's data if it does not exist already
            var master = await newUser.TryCreateUser("Orleans Master", "orleans.master");
            try
            {
                await _userRepository.AddUser(await newUser.GetUserState());
            }
            catch(Exception ex)
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
            await newUser.TryCreateUser(request.Name, request.Username);

            try
            {
                await _userRepository.AddUser(await newUser.GetUserState());
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }

            return Ok(request.Username);
        }

        public IActionResult RetriveChatsPreviewsBy(string username)
        {
            if (!String.IsNullOrEmpty(username))
            {
                throw new ArgumentException("...");
            }
            if (_userRepository.UserIsRegistered(username) == null)
            {
                throw new ArgumentException("...");
            }

            var userGrain = _grainFactory.GetGrain<IUser>(username);

            //var response = new List<ChatPreviewDTO>();  
            var response = new List<UserMessage>(); //here there is the chat in which the mess has been written

            var chats = userGrain.GetUserState().Result.Chats;

            foreach (var chat in chats)
            {
                var chatGrain = _grainFactory.GetGrain<IChatRoom>(chat);
                var lastmessage = chatGrain.GetChatState().Result.Messages.Last();
                //var chatPreview = new ChatPreviewDTO(chat, lastmessage);
                response.Add(lastmessage);
            }

            return Ok(response);
        }


        public async Task<IActionResult> RetriveUserFriends(string username)
        {
            if (!String.IsNullOrEmpty(username))
            {
                throw new ArgumentException("...");
            }
            if (_userRepository.UserIsRegistered(username) == null)
            {
                throw new ArgumentException("...");
            }

            var userGrain = _grainFactory.GetGrain<IUser>(username);

            return Ok(userGrain.GetUserState().Result.Friends);
        }

        

    }
}
