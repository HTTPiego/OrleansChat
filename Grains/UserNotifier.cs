

using GrainInterfaces;
using Grains.GrainState;
using Orleans.Runtime;

namespace Grains
{
    public class UserNotifier : Grain, IUserNotifier
    {

        private readonly IPersistentState<UserNotifierState> _state;

        public UserNotifier([PersistentState("state")] IPersistentState<UserNotifierState> state) 
        {
            _state = state;
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await _state.ReadStateAsync();
            await base.OnActivateAsync(cancellationToken);
        }

        public async Task ReceiveNotification(string notification)
        {
            _state.State.Notifications.Add(notification);
            await _state.WriteStateAsync();
            await Task.CompletedTask;
        }

        public Task<List<string>> RetriveNotifications()
        {
            return Task.FromResult(_state.State.Notifications);
        }
    }
}
