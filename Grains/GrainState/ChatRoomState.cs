using GrainInterfaces;
using Grains.DTOs;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Grains.GrainState
{
    [GenerateSerializer]
    public class ChatRoomState
    {

        [Id(0)]
        public string ChatName { get; set; }

        [Id(1)]
        public List<string> ChatRoomMembers = new();

        [Id(2)]
        public List<UserMessage> Messages = new();

        [Id(3)]
        public bool IsGroup { get; set; } = false;

        public async Task<ChatRoomDTO> GetChatRoomStateDTO()
        {
            return await Task.FromResult(new ChatRoomDTO(ChatName, Messages));
        }

        public ChatRoomDB ObtainChatRoomDB() 
        {
            return new ChatRoomDB(ChatName);
        }

    }

    
}
