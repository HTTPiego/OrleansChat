
using ChatSilo.DTOs;
using GrainInterfaces;

namespace Grains.DTOs
{
    [GenerateSerializer]
    public class ChatPreviewDTO(string ChatId, UserMessage LastMessage, bool IsGroup)
    {
        [Id(0)]
        public string ChatId { get; set; } = ChatId;
        [Id(1)]
        public UserMessage LastMessage { get; set; } = LastMessage;

        [Id(2)] 
        public bool IsGroup { get; set; } = IsGroup;

        [Id(3)] 
        public int PendingMessages { get; set; } = 0;
    }
}
