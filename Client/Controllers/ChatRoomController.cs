using GrainInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatRoomController : ControllerBase
    {
        // TODO: to do
        private readonly IGrainFactory _grainFactory;

        public ChatRoomController(IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }

        [HttpGet("{chatRoomId}/users")]
        public async Task<IActionResult> GetAllUsersForChatRoom(string chatRoomId)
        {
            var chatroom = _grainFactory.GetGrain<IChatRoom>(chatRoomId);
            var members = await chatroom.GetMembers();

            return Ok(new {Members = members});
        }
        
        [HttpGet("add/{username}")]
        public async Task<IActionResult> AddUserToChatRoom(int username)
        {
            //var newUser = _grainFactory.GetGrain<IUser>(username);
            return Ok($"Success: Client wants to add {username} to chat");
        }


    }
}
