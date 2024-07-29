using Proxel.Protocol.Types;
using System.IO;
using System.Threading.Tasks;

namespace Proxel.Protocol.Helpers
{
    public class PacketReader
    {
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
