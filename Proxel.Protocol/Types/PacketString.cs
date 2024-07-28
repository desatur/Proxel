using System.IO;
using System.Threading.Tasks;
using System;
using System.Text;
using Proxel.Protocol.Helpers;
using System.Net.Sockets;

namespace Proxel.Protocol.Types
{
    public static class PacketString
    {
        public static async Task<string> ReadStringAsync(Stream stream)
        {
            int length = await VarInt.ReadVarIntAsync(stream);
            byte[] data = new byte[length];
            int read = stream.Read(data, 0, length);
            if (read != length)
            {
                throw new Exception("Unexpected end of stream.");
            }
            return Encoding.UTF8.GetString(data);
        }

        public static async Task WriteStringAsync(Stream stream, string value)
        {
            byte[] stringBytes = Encoding.UTF8.GetBytes(value);
            await VarInt.WriteVarIntAsync(stream, stringBytes.Length);
            await stream.WriteAsync(stringBytes, 0, stringBytes.Length);
        }
    }
}
