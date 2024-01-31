using Grains.DTOs;
using Grains.GrainState;
using Orleans.Runtime;
using Grains;

namespace GrainInterfaces
{
    public interface IUser : IGrainWithStringKey //IGrainWithGuidKey
    {
        Task<UserDTO> TryCreateUser(string name, string username);
        Task JoinChatRoom(string chatRoomId);
        Task AddFriend(string username);
        Task SendMessage(string chatRoom, string message);

        Task<UserState> GetUserState();
        /*Task<UserDTO> GetUserStateDTO();
        
        Task<UserPersonalDataDTO> GetUserPersonalStateDTO();*/
    }
}
