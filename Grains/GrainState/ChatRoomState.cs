using ChatSilo.DTOs;
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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid IdChatRoom { get; set; }

        [Id(1)]
        public string ChatName { get; set; } = default!;

        [NotMapped]
        [Id(2)]
        public List<string> ChatRoomMembers = new();

        [NotMapped]
        [Id(3)]
        public List<UserMessage> Messages = new();

        [NotMapped]
        [Id(4)]
        public bool IsGroup { get; set; } = false;

        public async Task<ChatRoomDTO> GetChatRoomStateDTO()
        {
            return await Task.FromResult(new ChatRoomDTO(ChatName, ChatRoomMembers));
        }
    }
}
