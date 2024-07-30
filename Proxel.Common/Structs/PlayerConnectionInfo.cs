namespace Proxel.Protocol.Structs
{
    public readonly struct PlayerConnectionInfo
    {
        public ushort ProtocolVersion { get; }
        public string ServerAddress { get; }
        public ushort ServerPort { get; }

        public PlayerConnectionInfo(ushort protocolVersion, string serverAddress, ushort serverPort)
        {
            ProtocolVersion = protocolVersion;
            ServerAddress = serverAddress;
            ServerPort = serverPort;
        }
    }
}
