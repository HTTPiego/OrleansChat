using Client.Repositories;
using GrainInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatRoomsController : ControllerBase
    {
        // TODO:
        private readonly ChatRoomRepository _chatRoomRepository;

        private readonly IGrainFactory _grainFactory;

        private readonly ILogger<ChatRoomsController> _logger;

        public ChatRoomsController(ChatRoomRepository chatRoomRepository, IGrainFactory grainFactory, ILogger<ChatRoomsController> logger)
        {
            _chatRoomRepository = chatRoomRepository;
            _grainFactory = grainFactory;
            _logger = logger;   
        }

        /*[HttpGet("{chatRoomId}/users")]
        public async Task<IActionResult> GetAllUsersForChatRoom(string chatRoomId)
        {
            var chatroom = _grainFactory.GetGrain<IChatRoom>(chatRoomId);
            var members = await chatroom.GetMembers();

            return Ok(new {Members = members});
        }*/
        
        [HttpGet("add/{username}/to/{chatroom}")]
        public async Task<IActionResult> AddUserToChatRoom(string username, string chatroom)
        {
            var chat = _grainFactory.GetGrain<IChatRoom>(chatroom);
            await chat.AddUser(username);

            return Ok($"Success: Client wants to add {username} to chat");
        }



        [HttpGet("{username1}/befriend/{username2}")]
        public async Task<IActionResult> Befriend(string username1, string username2)
        {
            var user1 = _grainFactory.GetGrain<IUser>(username1);
            var user2 = _grainFactory.GetGrain<IUser>(username2);

            var newChatRoom = _grainFactory.GetGrain<IChatRoom>($"{username1}_{username2}");
            try
            {
                await _chatRoomRepository.AddChatRoom(await newChatRoom.GetChatState());
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }

            try
            {
                await user1.JoinChatRoom(newChatRoom.GetPrimaryKeyString());
                await user1.AddFriend(username2);
                await user2.JoinChatRoom(newChatRoom.GetPrimaryKeyString());
                await user2.AddFriend(username1);

                await newChatRoom.AddMultipleUsers(new List<string>() { username1, username2 });
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, $"An error occured while creating the user relationship.");
            }

            return Ok(new { Message = $"User {username1} has befriended {username2} successfully and a chat room has been created." });
        }

    }
}
