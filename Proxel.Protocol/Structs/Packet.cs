namespace Proxel.Protocol.Structs
{
    public readonly struct Packet
    {
        public byte PacketId { get; }
        public byte[] Data { get; }

        public ushort Length
        {
            get
            {
                return (ushort)(sizeof(int) + (Data?.Length ?? 0));
            }
        }

        public Packet(byte packetId, byte[] data)
        {
            PacketId = packetId;
            Data = data != null ? (byte[])data.Clone() : null;
        }
    }
}
