using Proxel.Protocol.Types;
using System;
using System.IO;
using System.Net.Sockets;
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
        public static ushort ReadUnsignedShort(Stream stream)
        {
            byte[] data = new byte[2];
            ushort read = (ushort)stream.Read(data, 0, 2);
            if (read != 2)
            {
                throw new Exception("Unexpected end of stream.");
            }
            return (ushort)((data[0] << 8) | data[1]);
        }
    }
}
