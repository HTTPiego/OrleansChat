using GrainInterfaces;
using System.Threading;

namespace Grains
{
    public class User : Grain, IUser
    {

        private string _userNickname;

        private IChatsManager _chatsManager;

        public override Task OnActivateAsync(CancellationToken ct)
        {
            _chatsManager = GrainFactory.GetGrain<IChatsManager>(Guid.NewGuid());
            return base.OnActivateAsync(ct);
        }

        public Task SetUserNickname(string nickname)
        {
            _userNickname = nickname;
            return Task.CompletedTask;
        }
        public Task<string> GetUserNickname()
        {
            return Task.FromResult(_userNickname);
        }
        public Task<IChatsManager> GetChatsManager()
        {
            return Task.FromResult(_chatsManager);
        }

        /*//TODO: ???
        public async Task<IUserNotifier> GetUserNotifier()
        {
            return await Task.FromResult(_userNotifier);
        }*/

        public Task ReceiveNotification(IChatNotificationsHandler userNotifier, string notification)
        {
            throw new NotImplementedException();
        }

    }
}
