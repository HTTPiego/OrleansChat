namespace Grains.GrainState
{
    [GenerateSerializer]
    public class UserNotifierState
    {
        [Id(0)]
        public readonly List<string> Notifications = new();
    }
}
