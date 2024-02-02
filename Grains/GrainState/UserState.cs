using Grains.DTOs;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grains.GrainState
{
    [GenerateSerializer]
    public class UserState
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Id(0)]
        public Guid IdChatRoom { get; set; }

        [Id(1)]
        public string Username { get; set; }

        [NotMapped]
        [Id(2)]
        public string Name { get; set; } = default!;

        [NotMapped]
        [Id(3)]
        public List<string> Chats = new();

        [NotMapped]
        [Id(4)]
        public List<string> Friends = new();

        public async Task<UserDTO> GetUserStateDTO()
        {
            return await Task.FromResult(new UserDTO(
                    name: Name,
                    username: Username,
                    chats: Chats,
                    friends: Friends
                ));
        }

        public async Task<UserPersonalDataDTO> GetUserPersonalStateDTO()
        {
            return await Task.FromResult(new UserPersonalDataDTO(Name, Username));
        }

    }
}
