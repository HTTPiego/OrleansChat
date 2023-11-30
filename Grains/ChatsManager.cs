using GrainInterfaces;

namespace Grains
{
    public class ChatsManager : Grain, IChatsManager
    {
        private readonly IGrainFactory _grainFactory;

        private List<IDirectChatRoom> _directs;

        private List<IGroupChatRoom> _groups;

        private List<string> _chatNotifications = new List<string>();

        public ChatsManager(IGrainFactory grainFactory) 
        {
            _grainFactory = grainFactory;
        }

        public Task<IGroupChatRoom> CreateGroupChat(IUser chatsManagerOwner, List<IUser> members)
        {
            IGroupChatRoom chat = _grainFactory.GetGrain<IGroupChatRoom>(Guid.NewGuid());
            foreach (var member in members) 
            {
                chat.addUser(chatsManagerOwner, member);
            }
            chat.addUser(chatsManagerOwner, chatsManagerOwner);
            return Task.FromResult(chat);
        }

        public Task<IDirectChatRoom> InitializeDirectChat(IUser chatsManagerOwner, IUser friend)
        {
            IDirectChatRoom chat = _grainFactory.GetGrain<IDirectChatRoom>(Guid.NewGuid());
            chat.addUser(chatsManagerOwner, friend);    
            chat.addUser(chatsManagerOwner, chatsManagerOwner);
            _directs.Add(chat);
            return Task.FromResult(chat);
        }

        public Task LeaveGroupChat(IUser chatsManagerOwner, IGroupChatRoom chat)
        {
            if (!_directs.Contains(chat))
            {
                return Task.FromException(new ArgumentException());
            }
            chat.removeUser(chatsManagerOwner, chatsManagerOwner);
            _groups.Remove(chat);
            return Task.CompletedTask;
        }

        public Task<List<string>> getMessages(IDirectChatRoom chat)
        {
            return chat.getMessages();
        }

        public Task ReceiveNotificationFrom(IDirectChatRoom directChat, string notification)
        {
            if (!_directs.Contains(directChat))
            {
                _directs.Add(directChat);
            }
            _chatNotifications.Add(notification);
            return Task.CompletedTask;
        }

        public Task ReceiveNotificationFrom(IGroupChatRoom groupChat, string notification)
        {
            if (!_directs.Contains(groupChat))
            {
                _groups.Add(groupChat);
            }
            _chatNotifications.Add(notification);
            return Task.CompletedTask;
        }

        /*public Task updateGroupsAfterRemoval(IGroupChatRoom groupChat, string notification)
        {
            _chatNotifications.Add(notification);
            _groups.Remove(groupChat);
            return Task.CompletedTask;
        }*/

    }
}
