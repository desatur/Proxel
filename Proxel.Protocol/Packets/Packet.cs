using Proxel.Protocol.Helpers;
using System.IO;
using System.Threading.Tasks;

namespace Proxel.Protocol.Packets
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

        public static async Task<Packet> ReadPacketAsync(Stream stream, bool readData = true)
        {
            int length = await VarInt.ReadVarIntAsync(stream);
            int packetId = await VarInt.ReadVarIntAsync(stream);
            byte[] data;
            if (readData)
            {
                int dataSize = length - VarInt.GetVarIntSize(packetId);
                data = new byte[dataSize];

                int bytesRead = 0;
                while (bytesRead < dataSize)
                {
                    int read = await stream.ReadAsync(data, bytesRead, dataSize - bytesRead);
                    if (read == 0) throw new IOException("Unexpected end of stream.");
                    bytesRead += read;
                }
            }
            else
            {
                data = [];
            }
            return new Packet(packetId, data);
        }
    }
}
