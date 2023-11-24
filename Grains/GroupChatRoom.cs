using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans.Utilities;
using System.Linq;

namespace Grains
{
    public class GroupChatRoom : Grain, IGroupChatRoom
    {
        private readonly ObserverManager<IChatNotificationsHandler> _chatNotificationsHandler;
        private IUser _groupCreator;
        private List<IUser> _admins = new List<IUser>();
        private List<IUser> _members = new List<IUser>();
        private List<string> _messages = new List<string>();

        //TODO: handle timer 
        public GroupChatRoom(ILogger<IChatNotificationsHandler> logger)
        {
            _chatNotificationsHandler = new ObserverManager<IChatNotificationsHandler>(TimeSpan.FromMinutes(5), logger);
        }

        public override Task OnActivateAsync(CancellationToken ct)
        {
            var chatNotificationsHandler = GrainFactory.GetGrain<IChatNotificationsHandler>(Guid.NewGuid());
            _chatNotificationsHandler.Subscribe(chatNotificationsHandler, chatNotificationsHandler);
            return base.OnActivateAsync(ct);
        }

        /*public Task PostMessage(string message, IUser messageAuthor)
        {
            if (message == String.Empty)
            {
                return Task.CompletedTask; //exception
            }
            _messages.Add(message);
            _chatNotificationsHandler.Notify(chatNotificationsHandler => chatNotificationsHandler.HandleNotificationFrom(this, "Message from " + messageAuthor.GetUserNickname()),
                                                chatNotificationsHandler => !chatNotificationsHandler.GetChatsManagers().Result
                                                                            .SkipWhile(mng => mng.Equals(messageAuthor.GetChatsManager().Result))
                                                                            .Contains(messageAuthor.GetChatsManager().Result));
            //observer => !observer.GetPrimaryKey().Equals(messageAuthor.GetPrimaryKey()));
            return Task.CompletedTask;
        }*/

        public Task PostMessage(string message, IUser messageAuthor)
        {
            if (message == String.Empty)
            {
                return Task.FromException(new Exception());
            }
            _messages.Add(message);
            _chatNotificationsHandler.Notify(chatNotificationsHandler => 
                                                chatNotificationsHandler.HandleNotificationFrom(this, "Message from " + messageAuthor.GetUserNickname(), messageAuthor));
            return Task.CompletedTask;
        }

        public Task<List<string>> getMessages()
        {
            return Task.FromResult(_messages);
        }

        public async Task addUser(IUser whoAdds, IUser newMember)
        {
            if (newMember == null || _members.Contains(newMember))
            {
                throw new ArgumentException();
            }
            _members.Add(newMember);
            await _chatNotificationsHandler.First().Subscribe(newMember.GetChatsManager().Result);
            await _chatNotificationsHandler.Notify(chatNotificationsHandler => chatNotificationsHandler
                                                                                .HandleNotificationFrom(this, whoAdds.GetUserNickname() + "added " + newMember.GetUserNickname(), whoAdds));
            return;
        }

        public async Task removeUser(IUser whoRemoves, IUser member)
        {
            if (member == null || !_members.Contains(member))
            {
                throw new ArgumentException();
            }
            _members.Remove(member);
            await _chatNotificationsHandler.Notify(chatNotificationsHandler => chatNotificationsHandler
                                                                                .HandleNotificationFrom(this, whoRemoves.GetUserNickname() + "removed " + member.GetUserNickname(), whoRemoves));
            await _chatNotificationsHandler.First().Unsubscribe(member.GetChatsManager().Result);
            return;
        }

        public Task setGroupCreator(IUser groupCreator)
        {
            // groupCreator == null
            if (_groupCreator != null)
            {
                return Task.FromException(new ArgumentException());
            }
            if (!_members.Contains(groupCreator))
            {
                return Task.FromException(new ArgumentException());
            }
            _groupCreator = groupCreator;
            return Task.CompletedTask;
        }

        public Task addAdmin(IUser whoAdds, IUser newAdmin)
        {
            // newAdmin == null
            if (!whoAdds.Equals(_groupCreator) 
                || !_admins.Contains(whoAdds))
            {
                return Task.FromException(new ArgumentException());
            }
            if (!_members.Contains(newAdmin))
            {
                return Task.FromException(new ArgumentException());
            }
            _admins.Add(newAdmin);
            return Task.CompletedTask;
        }

        public Task removeAdmin(IUser whoRemoves, IUser admin)
        {
            // newAdmin == null
            if (!whoRemoves.Equals(_groupCreator)
                || !_admins.Contains(whoRemoves))
            if (!_members.Contains(admin))
            {
                return Task.FromException(new ArgumentException());
            }
            _admins.Remove(admin);
            return Task.CompletedTask;
        }
    }
}
