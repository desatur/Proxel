namespace Proxel.Protocol.Structs
{
    public readonly struct Player
    {
        public string Name { get; }
        public string UUID { get; }
        public PlayerConnectionInfo ConnectionInfo { get; }

        public Player(string name, string uuid, PlayerConnectionInfo playerConnectionInfo)
        {
            Name = name;
            UUID = uuid;
            ConnectionInfo = playerConnectionInfo;
        }
    }

}
