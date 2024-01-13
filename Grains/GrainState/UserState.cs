using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grains.GrainState
{
    [Serializable]
    public class UserState
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid IdChatRoom { get; set; }
        
        public string Username { get; set; }

        [NotMapped]
        public string Name { get; set; } = default!;

        [NotMapped]
        public List<string> Chats = new();

        [NotMapped]
        public List<string> Friends = new();

    }
}
