namespace Grains.GrainState
{
    [GenerateSerializer]
    public class UserNotifierState
    {
        [Id(0)]
        public string OwnerUsername { get; set; }

        [Id(1)]
        public List<string> Notifications { get; set; } = new();
    }
}
