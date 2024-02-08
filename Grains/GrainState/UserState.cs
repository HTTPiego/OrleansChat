using Grains.DTOs;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grains.GrainState
{
    [GenerateSerializer]
    public class UserState
    {
        [Id(0)]
        public string Username { get; set; }

        [Id(1)]
        public string Name { get; set; } = default!;

        [Id(2)]
        public List<string> Chats { get; set; } = new();

        [Id(3)]
        public List<string> Friends { get; set; } = new();

        public async Task<UserDTO> GetUserStateDTO()
        {
            return await Task.FromResult(new UserDTO(
                    name: Name,
                    username: Username,
                    chats: Chats,
                    friends: Friends
                ));
        }

        /*public async Task<UserPersonalDataDTO> GetUserPersonalStateDTO()
        {
            return await Task.FromResult(new UserPersonalDataDTO(Name, Username));
        }*/

        public UserDB ObtainUserDB() 
        {
            return new UserDB(Username);
        }

    }

    /*public class UserDB
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid UserId { get; set; }

        public string Username { get; set; } = default!;

        public UserDB() { }
        
        public UserDB(string username) 
        {
            UserId = new Guid();
            Username = username;
        }

    }*/
}
