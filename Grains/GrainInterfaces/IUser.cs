using Grains.DTOs;
using Grains.GrainState;
using Orleans.Runtime;
using Grains;

namespace GrainInterfaces
{
    public interface IUser : IGrainWithStringKey //IGrainWithGuidKey
    {
        Task<UserDTO> TryCreateUserRetDTO(string name, string username);
        Task<UserState> TryCreateUserRetState(string name, string username);
        Task JoinChatRoom(string chatRoomId);
        Task AddFriend(string username);
        Task SendMessage(UserMessage message);

        Task<IPersistentState<UserState>> GetUserState();

        Task<UserPersonalDataDTO> GetUserPersonalStateDTO();

        Task<string> GetUsername();

        Task<List<string>> GetUserFriends();

        Task<List<string>> GetUserChats();
        Task<UserDB> ObtainUserDB();
        /*Task<UserDTO> GetUserStateDTO();
        
        Task<UserPersonalDataDTO> GetUserPersonalStateDTO();*/
    }
}
