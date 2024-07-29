using System;
using System.IO;
using System.Threading.Tasks;

namespace Proxel.Protocol.Helpers
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
                    await stream.WriteAsync([(byte)value], 0, 1);
                    return;
                }
                byte toWrite = (byte)(value & SEGMENT_BITS | CONTINUE_BIT);
                await stream.WriteAsync([toWrite], 0, 1);
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
}
