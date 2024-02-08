

using GrainInterfaces;
using Grains.DTOs;
using Grains.GrainState;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;

namespace Grains
{
    public class UserNotifier : Grain, IUserNotifier
    {
        private readonly ILogger<UserNotifier> _logger;
        private IPersistentState<UserNotifierState> _state;

        public UserNotifier([PersistentState("state")] IPersistentState<UserNotifierState> state,
                            ILogger<UserNotifier> logger) 
        {
            _state = state;
            _logger = logger;
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await _state.ReadStateAsync();
            await base.OnActivateAsync(cancellationToken);
        }

        public async Task ReceiveNotification(string notification)
        {
            _logger.LogCritical("ho ricevuto la notifica di @#@#@#@#@#@#@ ======>" + _state.State.OwnerUsername);
            Console.WriteLine("ho ricevuto la notifica di " + _state.State.OwnerUsername);
            _state.State.Notifications.Add(notification);
            await _state.WriteStateAsync();
        }

        public Task<List<string>> RetriveNotifications()
        {
            return Task.FromResult(_state.State.Notifications);
        }

        public Task<string> GetOwnerUsername()
        {
            return Task.FromResult(_state.State.OwnerUsername);
        }

        public async Task TrySaveNotifier(string ownerusername)
        {
            if (String.IsNullOrEmpty(_state.State.OwnerUsername))
            {
                _state.State.OwnerUsername = ownerusername;

                await _state.WriteStateAsync();

                _logger.LogInformation($"{ownerusername}'s data has been persisted.");
            }
            else
            {
                _logger.LogWarning($"{ownerusername} already exists.");
            }
        }

    }
}
