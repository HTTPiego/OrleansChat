using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grains.GrainState
{
    [Serializable]
    public class ChatRoomState
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid IdChatRoom { get; set; }
        
        public string ChatName { get; set; } = default!;

        [NotMapped]
        public List<string> ChatRoomMembers = new();

        [NotMapped]
        public List<string> Messages = new();

        [NotMapped]
        public bool IsGroup { get; set; } = false;
    }
}
