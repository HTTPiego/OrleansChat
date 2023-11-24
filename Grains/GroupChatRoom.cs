using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans.Utilities;
using System.Linq;

namespace Grains
{
    public class GroupChatRoom : Grain, IGroupChatRoom
    {
        private readonly ObserverManager<IChatNotificationsHandler> _chatNotificationsHandler;
        //private IChatNotificationsHandler _chatNotificationsHandler;
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
                return Task.CompletedTask; //exception
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
                return; //exception  
            }
            _members.Add(newMember);
            await _chatNotificationsHandler.First().Subscribe(newMember.GetChatsManager().Result);
            await _chatNotificationsHandler.Notify(observer => observer.HandleNotificationFrom(this, "You have been added to new group by " + whoAdds.GetUserNickname(), whoAdds),
                                                    observer => !whoAdds.Equals(newMember));
            return;
        }

        public async Task removeUser(IUser whoRemoves, IUser member)
        {
            if (member == null || !_members.Contains(member))
            {
                return; //exception  
            }
            _members.Remove(member);
            await _chatNotificationsHandler.First().Unsubscribe(member.GetChatsManager().Result);
            await _chatNotificationsHandler.Notify(observer => observer.HandleNotificationFrom(this, "You have been removed by " + whoRemoves.GetUserNickname(), whoRemoves),
                                                    observer => !whoRemoves.Equals(member));
            return;
        }

        public async Task setGroupCreator(IUser groupCreator)
        {
            // groupCreator == null
            if (_groupCreator != null)
            {
                return; //error
            }
            if (!_members.Contains(groupCreator))
            {
                await addUser(groupCreator, groupCreator);
            }
            _groupCreator = groupCreator;
            throw new NotImplementedException();
        }

        public async Task addAdmin(IUser whoAdds, IUser newAdmin)
        {
            // newAdmin == null
            if (!whoAdds.Equals(_groupCreator) 
                || !_admins.Contains(whoAdds) 
                || !whoAdds.Equals(_groupCreator))
            {
                return; //exception  
            }
            if (!_members.Contains(newAdmin))
            {
                await addUser(whoAdds, newAdmin);
            }
            if (_members.Contains(newAdmin))
            {
                return; //exception  
            }
            _admins.Add(newAdmin);
            return;
        }

        public Task removeAdmin(IUser whoRemoves, IUser admin)
        {
            // newAdmin == null
            if (!whoRemoves.Equals(_groupCreator)
                || !_admins.Contains(whoRemoves)
                || !whoRemoves.Equals(_groupCreator))
            if (!_members.Contains(admin))
            {
               return Task.CompletedTask;
            }
            _admins.Remove(admin);
            return Task.CompletedTask;
        }
    }
}
