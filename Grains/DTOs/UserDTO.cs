namespace Grains.DTOs;

[GenerateSerializer]
public class UserDTO(string name, string username, List<string> chats, List<string> friends)
{
    [Id(0)]
    public string name { get; set; } = name;
    [Id(1)]
    public string username { get; set; } = username;
    [Id(2)]
    public List<string> chats { get; set; } = chats;
    [Id(3)]
    public List<string> friends { get; set; } = friends;
}