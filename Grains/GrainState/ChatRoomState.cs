using System.ComponentModel.DataAnnotations.Schema;

namespace Grains.GrainState
{
    [Serializable]
    public class ChatRoomState
    {
        public string ChatName { get; set; } = default!;

        [NotMapped]
        public List<string> ChatRoomMembers = new();

        [NotMapped]
        public List<string> Messages = new();

        [NotMapped]
        public bool IsGroup { get; set; } = false;
    }
}
