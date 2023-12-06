using GrainInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly IGrainFactory _grainFactory;
        
        public UsersController(IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }

        [HttpGet("new")]
        public async Task<IActionResult> CreateUser(string username)
        {
            //var newUser = _grainFactory.GetGrain<IUser>(username);
            return Ok($"Success: Received username {username}");
        }

        [HttpGet("{username1}/befriend/{username2}")]
        public async Task<IActionResult> Befriend(string username1, string username2)
        {
            var user1 = _grainFactory.GetGrain<IUser>(username1);
            var user2 = _grainFactory.GetGrain<IUser>(username2);
            
            var newChatRoom = _grainFactory.GetGrain<IChatRoom>($"{username1}_{username2}");

            try
            {
                await user1.JoinChatRoom(newChatRoom.GetPrimaryKeyString());
                await user2.JoinChatRoom(newChatRoom.GetPrimaryKeyString());
                await newChatRoom.AddMultipleUsers(new List<string>(){username1, username2});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, $"An error occured while creating the user relationship.");
            }

            return Ok($"User {username1} has befriended {username2} successfully and a chat room has been created.");
        }

    }
}
