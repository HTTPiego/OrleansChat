using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grains.DTOs
{
    [GenerateSerializer]
    public class ChatRoomDTO(string chatName, List<string> messages)
    {
        [Id(0)]
        public string ChatName { get; set; } = chatName;

        [Id(1)] 
        public List<string> Messages { get; set; } = messages; 

    }
}
