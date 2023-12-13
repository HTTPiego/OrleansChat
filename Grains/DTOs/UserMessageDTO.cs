namespace ChatSilo.DTOs;

public class UserMessageDTO
{
    public string Username { get; set; }
    public Guid ChatRoomId { get; set; }
    public string TextMessage { get; set; }
    public DateTime Timestamp { get; set; }
}