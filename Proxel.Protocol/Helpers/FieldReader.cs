using System.IO;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Proxel.Protocol.Helpers
{
    public static class FieldReader
    {
        public static async Task<string> ReadStringAsync(Stream stream)
        {
            int length = await VarInt.ReadVarIntAsync(stream);
            byte[] data = new byte[length];
            int read = stream.Read(data, 0, length);
            if (read != length)
            {
                throw new IOException("Unexpected end of stream.");
            }
            return Encoding.UTF8.GetString(data);
        }

        public static async Task<string> ReadUuidAsync(Stream stream)
        {
            static string FormatUuid(ulong mostSigBits, ulong leastSigBits)
            {
                // Convert mostSigBits and leastSigBits to byte arrays
                byte[] mostSigBytes = BitConverter.GetBytes(mostSigBits);
                byte[] leastSigBytes = BitConverter.GetBytes(leastSigBits);

                // Arrange the bytes in the UUID format
                StringBuilder sb = new StringBuilder(36);
                sb.AppendFormat("{0:x2}{1:x2}{2:x2}{3:x2}-", mostSigBytes[0], mostSigBytes[1], mostSigBytes[2], mostSigBytes[3]);
                sb.AppendFormat("{0:x2}{1:x2}-", mostSigBytes[4], mostSigBytes[5]);
                sb.AppendFormat("{0:x2}{1:x2}-", mostSigBytes[6], mostSigBytes[7]);
                sb.AppendFormat("{0:x2}{1:x2}{2:x2}{3:x2}{4:x2}{5:x2}{6:x2}{7:x2}", leastSigBytes[0], leastSigBytes[1], leastSigBytes[2], leastSigBytes[3], leastSigBytes[4], leastSigBytes[5], leastSigBytes[6], leastSigBytes[7]);

                return sb.ToString();
            }

            byte[] uuidBytes = new byte[16];
            int bytesRead = await stream.ReadAsync(uuidBytes, 0, uuidBytes.Length);
            if (bytesRead != uuidBytes.Length)
            {
                throw new IOException("Unable to read the full UUID from the stream.");
            }
            ulong mostSigBits = BitConverter.ToUInt64(uuidBytes, 0);
            ulong leastSigBits = BitConverter.ToUInt64(uuidBytes, 8);
            return FormatUuid(mostSigBits, leastSigBits);
        }
        public static ushort ReadUnsignedShort(Stream stream)
        {
            byte[] data = new byte[2];
            int read = stream.Read(data, 0, 2);
            if (read != 2)
            {
                throw new Exception("Unexpected end of stream.");
            }
            return (ushort)((data[0] << 8) | data[1]);
        }

        public static async Task<byte[]> ReadByteArrayAsync(Stream stream)
        {
            int length = await VarInt.ReadVarIntAsync(stream);
            byte[] data = new byte[length];
            int bytesRead = 0;
            while (bytesRead < length)
            {
                int read = await stream.ReadAsync(data, bytesRead, length - bytesRead);
                if (read == 0)
                {
                    throw new IOException("Unexpected end of stream.");
                }
                bytesRead += read;
            }
            return data;
        }
    }
}
