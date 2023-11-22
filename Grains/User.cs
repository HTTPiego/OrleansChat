using GrainInterfaces;

namespace Grains
{
    public class User : IUser
    {

        //Notifier UserNotifier ...

        private string _userNickname;

        private List<IChatRoom> _chatRooms;

        //public GrainFactory grainFactory;

        /*public User(string nickname)
        {
            _userNickname = nickname;
        }*/

        public Task<string> GetUserNickname()
        {
            return Task.FromResult(this._userNickname);
        }

        public Task<IChatRoom> CreateGroupChat(IUser groupCreator, List<IUser> members, IGrainFactory grainFactory)
        {
            IChatRoom chat = grainFactory.GetGrain<IChatRoom>(Guid.NewGuid());
            foreach (var member in members)
            {
                chat.addUser(member);
            }
            chat.addUser(groupCreator);
            return Task.FromResult(chat);
        }

        public Task<IChatRoom> InitializeChat(IUser whoStartedTheChat, IUser friend, IGrainFactory grainFactory)
        {
            IChatRoom chat = grainFactory.GetGrain<IChatRoom>(Guid.NewGuid());
            chat.addUser(friend);
            chat.addUser(whoStartedTheChat);
            return Task.FromResult(chat);
        }

        public Task LeaveGroupChat(IChatRoom chat)
        {
            if (!_chatRooms.Contains(chat))
            {
                return Task.CompletedTask; //exception
            }
            chat.removeUser(this); //TODO: handle user permissions
            _chatRooms.Remove(chat);
            return Task.CompletedTask;
        }

        public Task<List<string>> readMessages(IChatRoom chat)
        {
            return chat.getMessages();
        }

        public Task ReceiveNotificationFrom(string notification, IChatRoom chat)
        {
            Console.WriteLine(notification); //TODO: handle notification
            if (!_chatRooms.Contains(chat))
            {
                _chatRooms.Add(chat);
            }
            return Task.CompletedTask;
        }
    }
}
