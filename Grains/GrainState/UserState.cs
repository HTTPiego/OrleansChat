namespace Grains.GrainState;

[Serializable]
public class UserState
{
    public string Name { get; set; } = default!;
    
    public string Username { get; set; }
    
    public List<string> Chats = new();
    
    public List<string> Friends = new();

}