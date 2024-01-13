using Client.Repositories;
using Client.Repositories.Interfaces;
using GrainInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatRoomsController : ControllerBase
    {
        // TODO:
        private readonly IChatRoomRepository _chatRoomRepository;

        private readonly IGrainFactory _grainFactory;

        public ChatRoomsController(ChatRoomRepository chatRoomRepository, IGrainFactory grainFactory)
        {
            _chatRoomRepository = chatRoomRepository;
            _grainFactory = grainFactory;
        }

        [HttpGet("{chatRoomId}/users")]
        public async Task<IActionResult> GetAllUsersForChatRoom(string chatRoomId)
        {
            /*// REDIS DB CONNECTION
            ConnectionMultiplexer connection = ConnectToRedis();
            IDatabase db = connection.GetDatabase();
            IServer srv = connection.GetServer("localhost", 6379);

            //GET ALL GRAINS KEYS IN REDIS
            var keys = srv.Keys(db.Database, $"default/state/chatroom/*").ToArray();

            Console.Write(keys.First());*/


            var chatroom = _grainFactory.GetGrain<IChatRoom>(chatRoomId);
            var members = await chatroom.GetMembers();

            return Ok(new {Members = members});
        }
        
        [HttpGet("add/{username}/to/{chatroom}")]
        public async Task<IActionResult> AddUserToChatRoom(string username, string chatroom)
        {
            var chat = _grainFactory.GetGrain<IChatRoom>(chatroom);
            await chat.AddUser(username);

            return Ok($"Success: Client wants to add {username} to chat");
        }

        private ConnectionMultiplexer ConnectToRedis()
        {
            ConfigurationOptions options = new ConfigurationOptions { EndPoints = { { "localhost", 6379 } } };
            return ConnectionMultiplexer.Connect(options);
        }
    }
}
