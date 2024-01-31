using System.Collections.Concurrent;
using Grains.DTOs;
using Client.Contracts;
using GrainInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;
using Client.Repositories.Interfaces;
using Client.Repositories;
using Grains;
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
            // REDIS DB CONNECTION
            ConnectionMultiplexer connection = ConnectToRedis();
            IDatabase db = connection.GetDatabase();
            IServer srv = connection.GetServer("localhost",6379);
                
            //GET ALL GRAINS KEYS IN REDIS
            var keys = srv.Keys(db.Database, $"default/state/user/*").ToArray();
            ConcurrentBag<UserPersonalDataDTO> users = new ConcurrentBag<UserPersonalDataDTO>();
            
            await Parallel.ForEachAsync(keys, async (key, ct) =>
            {
                var username = key.ToString().Split("/")[3];
                var userGrain = _grainFactory.GetGrain<IUser>(username);

                var grainState = await userGrain.GetUserPersonalState();
                
                users.Add(grainState);
            });
            
            return Ok(users.ToArray());
        }

        [HttpGet()]
        public async Task<IActionResult> SearchUsersByUsername([FromQuery(Name = "search")] string search)
        {
            //grain to keep lists in its state of all users id??

            Console.WriteLine(search);
            // REDIS DB CONNECTION
            ConnectionMultiplexer connection = ConnectToRedis();
            IDatabase db = connection.GetDatabase();
            IServer srv = connection.GetServer("localhost",6379);
                
            //GET ALL GRAINS KEYS IN REDIS
            var keys = srv.Keys(db.Database, $"default/state/user/*{search}*/state").ToArray();
            ConcurrentBag<UserPersonalDataDTO> users = new ConcurrentBag<UserPersonalDataDTO>();
            
            await Parallel.ForEachAsync(keys, async (key, ct) =>
            {
                var username = key.ToString().Split("/")[3];
                var userGrain = _grainFactory.GetGrain<IUser>(username);

                var grainState = await userGrain.GetUserPersonalState();
                
                users.Add(grainState);
            });
            
            return Ok(users.ToArray());
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

        public async Task<List<string>> RetriveUserChats(string userName)
        {
            if (!String.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("...");    
            }
            if (_userRepository.UserIsRegistered(userName) == null)
            {
                throw new ArgumentException("...");
            }

            var userGrain = _grainFactory.GetGrain<IUser>(userName);

            return await Task.FromResult(userGrain.GetUserState().Result.Chats);
        }

        public async Task<List<string>> RetriveUserFriends(string userName)
        {
            if (!String.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("...");
            }
            if (_userRepository.UserIsRegistered(userName) == null)
            {
                throw new ArgumentException("...");
            }

            var userGrain = _grainFactory.GetGrain<IUser>(userName);

            return await Task.FromResult(userGrain.GetUserState().Result.Friends);
        }

        /*[HttpGet("{username1}/befriend/{username2}")]
        public async Task<IActionResult> Befriend(string username1, string username2)
        {
            var user1 = _grainFactory.GetGrain<IUser>(username1);
            var user2 = _grainFactory.GetGrain<IUser>(username2);
            
            var newChatRoom = _grainFactory.GetGrain<IChatRoom>($"{username1}_{username2}");
            try
            {
                await _
            }

            try
            {
                await user1.JoinChatRoom(newChatRoom.GetPrimaryKeyString());
                await user1.AddFriend(username2);
                await user2.JoinChatRoom(newChatRoom.GetPrimaryKeyString());
                await user2.AddFriend(username1);
                
                await newChatRoom.AddMultipleUsers(new List<string>(){username1, username2});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, $"An error occured while creating the user relationship.");
            }

            return Ok(new{Message = $"User {username1} has befriended {username2} successfully and a chat room has been created."});
        }*/

        private ConnectionMultiplexer ConnectToRedis()
        {
            ConfigurationOptions options = new ConfigurationOptions { EndPoints = { {"localhost", 6379} } };
            return ConnectionMultiplexer.Connect(options);
        }

    }
}
