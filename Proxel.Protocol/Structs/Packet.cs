namespace Proxel.Protocol.Structs
{
    public readonly struct Packet
    {
        public int PacketId { get; }
        public byte[] Data { get; }

        public int Length
        {
            get
            {
                return sizeof(int) + (Data?.Length ?? 0);
            }
        }

        public Packet(int packetId, byte[] data)
        {
            PacketId = packetId;
            Data = data != null ? (byte[])data.Clone() : null;
        }
    }
}
