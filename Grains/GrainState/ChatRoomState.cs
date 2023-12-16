namespace Grains.GrainState
{
    [Serializable]
    public class ChatRoomState
    {
        public string ChatName { get; set; } = default!;

        public List<string> ChatRoomMembers = new();

        public List<string> Messages = new();
        public bool IsGroup { get; set; } = false;
    }
}
