using System.ComponentModel.DataAnnotations.Schema;

namespace Grains.GrainState
{
    [Serializable]
    public class UserState
    {
        public string Name { get; set; } = default!;

        [NotMapped]
        public string Username { get; set; }

        [NotMapped]
        public List<string> Chats = new();

        [NotMapped]
        public List<string> Friends = new();

    }
}
