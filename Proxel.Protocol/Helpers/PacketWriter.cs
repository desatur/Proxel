using Proxel.Protocol.Types;
using System.IO;
using System.Threading.Tasks;

namespace Proxel.Protocol.Helpers
{
    public class PacketWriter
    {
        public static async Task WritePacketAsync(Stream stream, Packet packet)
        {
            int packetIdSize = VarInt.GetVarIntSize(packet.PacketId);
            int dataSize = packet.Data.Length;
            int length = packetIdSize + dataSize;

            using (var buffer = new MemoryStream())
            {
                await VarInt.WriteVarIntAsync(buffer, length);
                await VarInt.WriteVarIntAsync(buffer, packet.PacketId);
                await buffer.WriteAsync(packet.Data, 0, dataSize);
                byte[] packetBytes = buffer.ToArray();
                await stream.WriteAsync(packetBytes, 0, packetBytes.Length);
            }
        }

        public static async Task WriteCustomPacketAsync(Stream stream, int packetId, byte[] data)
        {
            int packetIdSize = VarInt.GetVarIntSize(packetId);
            int dataSize = data.Length;
            int length = packetIdSize + dataSize;

            using (var buffer = new MemoryStream())
            {
                await VarInt.WriteVarIntAsync(buffer, length);
                await VarInt.WriteVarIntAsync(buffer, packetId);
                await buffer.WriteAsync(data, 0, dataSize);
                byte[] packetBytes = buffer.ToArray();
                await stream.WriteAsync(packetBytes, 0, packetBytes.Length);
            }
        }

        public static async Task WriteStringPacketAsync(Stream stream, int packetId, byte[] data)
        {
            int dataSize = data.Length;
            using (var buffer = new MemoryStream())
            {
                int lengthFieldSize = VarInt.GetVarIntSize(dataSize);
                await VarInt.WriteVarIntAsync(buffer, dataSize + VarInt.GetVarIntSize(packetId));
                await VarInt.WriteVarIntAsync(buffer, packetId);
                await buffer.WriteAsync(data, 0, dataSize);

                byte[] packetBytes = buffer.ToArray();
                await stream.WriteAsync(packetBytes, 0, packetBytes.Length);
            }
        }

    }
}
