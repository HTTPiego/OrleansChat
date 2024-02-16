using Client.Repositories;
using Client.Repositories.Interfaces;
using Client.SignalR;
using GrainInterfaces;
using Grains;
using Grains.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatRoomsController : ControllerBase
    {
        // TODO:
        private readonly IChatRoomRepository _chatRoomRepository;

        private readonly IGrainFactory _grainFactory;

        private readonly ILogger<ChatRoomsController> _logger;

        public ChatRoomsController(IChatRoomRepository chatRoomRepository, 
            IGrainFactory grainFactory, 
            ILogger<ChatRoomsController> logger)
        {
            _chatRoomRepository = chatRoomRepository;
            _grainFactory = grainFactory;
            _logger = logger;   
        }
        
        /*[HttpGet]
        public async Task<IActionResult> SendMessageToClient()
        {
            await _chatHub.Clients.All.SendAsync("chatRoomUpdate", "This is the new message");
            return Ok($"Success: Message has been sent to client in frontend");
        }*/
        
        [HttpGet("add/{username}/to/{chatroom}")]
        public async Task<IActionResult> AddUserToChatRoom(string username, string chatroom)
        {
            var chat = _grainFactory.GetGrain<IChatRoom>(chatroom);
            await chat.AddUser(username);

            return Ok($"Success: Client wants to add {username} to chat");
        }


        [HttpGet("{chatname}/get-messages")]
        public async Task<IActionResult> GetMessages(string chatname)
        {
            var chat = _grainFactory.GetGrain<IChatRoom>(chatname);
            var messages = await chat.GetMessages();
            
            
            return Ok(messages);
        }
        
        [HttpGet("{chatname}")]
        public IActionResult GetCompleteChatRoom(string chatname)
        {
            ChatRoomDTO chat;
            if (_chatRoomRepository.GetChatRoomBy(chatname) == null)
            {
                return StatusCode(400, "Chatname does not exist");
            }
            
            var chatGrain = _grainFactory.GetGrain<IChatRoom>(chatname);
            chat =  chatGrain.GetChatRoomStateDTO().Result;
            
            return Ok(chat);
        }


        [HttpGet("{username1}/befriend/{username2}")]
        public async Task<IActionResult> Befriend(string username1, string username2)
        {
            var user1 = _grainFactory.GetGrain<IUser>(username1);
            var user2 = _grainFactory.GetGrain<IUser>(username2);

            var chatName = $"{username1}_{username2}";
            var newChatRoom = _grainFactory.GetGrain<IChatRoom>(chatName);
            await newChatRoom.TrySaveChat(chatName);
            try
            {
                //await newChatRoom.AddMultipleUsers(new List<string> { username1, username2});
                await newChatRoom.ObtainChatRoomDB().ContinueWith(chatdb => _chatRoomRepository.AddChatRoom(chatdb.Result));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }

            try
            {
                await user1.JoinChatRoom(chatName);
                await user1.AddFriend(username2);
                await user2.JoinChatRoom(chatName);
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
