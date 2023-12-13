namespace Grains.DTOs;

[GenerateSerializer]
public class UserPersonalDataDTO(string name, string username)
{
    [Id(0)]
    public string name { get; set; } = name;
    [Id(1)]
    public string username { get; set; } = username;
}