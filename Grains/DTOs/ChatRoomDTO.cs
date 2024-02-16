using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrainInterfaces;

namespace Grains.DTOs
{
    [GenerateSerializer]
    public class ChatRoomDTO(string chatName, List<UserMessage> messages)
    {
        [Id(0)]
        public string ChatName { get; set; } = chatName;

        [Id(1)] 
        public List<UserMessage> Messages { get; set; } = messages; 

    }
}
