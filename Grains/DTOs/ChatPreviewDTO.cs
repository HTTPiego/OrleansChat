
using ChatSilo.DTOs;
using GrainInterfaces;

namespace Grains.DTOs
{
    [GenerateSerializer]
    public class ChatPreviewDTO(string ChatName, UserMessage LastMessage)
    {
        [Id(0)]
        public string ChatName { get; set; } = ChatName;
        [Id(1)]
        public UserMessage LastMessage { get; set; } = LastMessage;

    }
}
