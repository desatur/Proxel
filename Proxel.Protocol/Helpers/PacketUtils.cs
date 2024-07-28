using System.IO;
using System.Threading.Tasks;
using Proxel.Protocol.Types;

namespace Proxel.Protocol.Helpers
{
    public class PacketUtils
    {
        public static async Task<Stream> CreatePacketStream(Packet packet)
        {
            var stream = new MemoryStream();

            byte[] lengthBuffer = [];
            int lengthFieldSize = VarInt.GetVarIntSize(packet.Length);
            int packetLength = packet.Length;
            using (var lengthStream = new MemoryStream())
            {
                await VarInt.WriteVarIntAsync(lengthStream, packetLength);
                lengthBuffer = lengthStream.ToArray();
            }

            // Write the packet ID field
            byte[] packetIdBuffer;
            using (var packetIdStream = new MemoryStream())
            {
                await VarInt.WriteVarIntAsync(packetIdStream, packet.PacketId);
                packetIdBuffer = packetIdStream.ToArray();
            }

            // Write the data field
            byte[] dataBuffer = packet.Data;

            // Write length, packet ID, and data to the main stream
            await stream.WriteAsync(lengthBuffer, 0, lengthBuffer.Length);
            await stream.WriteAsync(packetIdBuffer, 0, packetIdBuffer.Length);
            await stream.WriteAsync(dataBuffer, 0, dataBuffer.Length);

            stream.Position = 0;
            return stream;
        }
    }
}
