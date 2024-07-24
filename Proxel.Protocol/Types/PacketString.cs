using System.IO;
using System.Threading.Tasks;
using System;
using System.Text;

namespace Proxel.Protocol.Types
{
    public static class PacketString
    {
        public static async Task<string> ReadStringAsync(Stream stream)
        {
            McPacket packet = await McPacket.ReadPacketAsync(stream);
            Stream packetStream = await McPacket.CreatePacketStream(packet);

            int length = await VarInt.ReadVarIntAsync(packetStream);
            if (length < 0 || length > 32767 * 3 + 3)
            {
                throw new InvalidOperationException("Invalid string length.");
            }

            byte[] buffer = new byte[length];
            int bytesRead = 0;

            while (bytesRead < length)
            {
                int read = await packetStream.ReadAsync(buffer, bytesRead, length - bytesRead);
                if (read == 0)
                {
                    throw new EndOfStreamException("Unexpected end of stream while reading string.");
                }
                bytesRead += read;
            }
            return Encoding.UTF8.GetString(buffer, 0, length);
        }

        public static async Task WriteStringAsync(Stream stream, string value)
        {
            byte[] stringBytes = Encoding.UTF8.GetBytes(value);
            await VarInt.WriteVarIntAsync(stream, stringBytes.Length);
            await stream.WriteAsync(stringBytes, 0, stringBytes.Length);
        }
    }
}
