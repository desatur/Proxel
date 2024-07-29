namespace Proxel.Protocol.Structs
{
    public readonly struct Player
    {
        public string Name { get; }
        public string UUID { get; }

        public Player(string name, string uuid)
        {
            Name = name;
            UUID = uuid;
        }
    }

}
