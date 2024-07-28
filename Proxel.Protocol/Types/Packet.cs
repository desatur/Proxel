namespace Proxel.Protocol.Types
{
    public class Packet
    {
        public int Length
        {
            get
            {
                return sizeof(int) + (Data != null ? Data.Length : 0);
            }
        }
        public int PacketId { get; set; }
        public byte[] Data { get; set; }

        public Packet(int packetId, byte[] data)
        {
            PacketId = packetId;
            Data = data;
        }
    }
}
