using System;
using System.IO;
using System.Threading.Tasks;

namespace Proxel.Protocol.Types
{
    public static class VarInt
    {
        private const int SEGMENT_BITS = 0x7F;
        private const int CONTINUE_BIT = 0x80;

        public static async Task<int> ReadVarIntAsync(Stream stream)
        {
            int value = 0;
            int position = 0;
            byte currentByte;

            while (true)
            {
                currentByte = (byte)stream.ReadByte();
                value |= (currentByte & SEGMENT_BITS) << position;

                if ((currentByte & CONTINUE_BIT) == 0) break;

                position += 7;

                if (position >= 32)
                {
                    throw new Exception("VarInt is too big");
                }
            }
            return value;
        }

        public static async Task WriteVarIntAsync(Stream stream, int value)
        {
            while (true)
            {
                if ((value & ~SEGMENT_BITS) == 0)
                {
                    await stream.WriteAsync(new[] { (byte)value }, 0, 1);
                    return;
                }
                byte toWrite = (byte)((value & SEGMENT_BITS) | CONTINUE_BIT);
                await stream.WriteAsync(new[] { toWrite }, 0, 1);
                value >>= 7;
            }
        }

        public static int GetVarIntSize(int value)
        {
            int size = 0;
            do
            {
                value >>= 7;
                size++;
            } while (value != 0);
            return size;
        }
    }

    public static class StreamExtensions
    {
        public static async Task<int> ReadByteAsync(this Stream stream)
        {
            byte[] buffer = new byte[1];
            int read = await stream.ReadAsync(buffer, 0, 1);
            return read == 0 ? -1 : buffer[0];
        }

        public static async Task WriteByteAsync(this Stream stream, byte value)
        {
            byte[] buffer = new byte[1] { value };
            await stream.WriteAsync(buffer, 0, 1);
        }
    }
}
