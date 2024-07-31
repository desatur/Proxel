namespace Proxel.Protocol.Structs
{
    public readonly struct Packet(byte packetId, byte[] data)
    {
        public byte PacketId { get; } = packetId;
        public byte[] Data { get; } = data != null ? (byte[])data.Clone() : null;

        public ushort Length
        {
            get
            {
                return (ushort)(sizeof(int) + (Data?.Length ?? 0));
            }
        }
    }
}
