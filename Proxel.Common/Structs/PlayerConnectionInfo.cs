namespace Proxel.Protocol.Structs
{
    public readonly struct PlayerConnectionInfo(ushort protocolVersion, string serverAddress, ushort serverPort)
    {
        public ushort ProtocolVersion { get; } = protocolVersion;
        public string ServerAddress { get; } = serverAddress;
        public ushort ServerPort { get; } = serverPort;
    }
}
