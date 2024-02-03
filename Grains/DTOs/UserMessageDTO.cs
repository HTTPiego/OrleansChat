namespace ChatSilo.DTOs;

public class UserMessageDTO
{
    public string AuthorUsername { get; set; }
    public string ChatRoomName { get; set; }
    public string TextMessage { get; set; }
    public DateTime Timestamp { get; set; }
}