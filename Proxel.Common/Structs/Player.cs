namespace Proxel.Protocol.Structs
{
    public readonly struct Player(string name, string uuid, PlayerConnectionInfo playerConnectionInfo)
    {
        public string Name { get; } = name;
        public string UUID { get; } = uuid;
        public PlayerConnectionInfo ConnectionInfo { get; } = playerConnectionInfo;
    }

}
