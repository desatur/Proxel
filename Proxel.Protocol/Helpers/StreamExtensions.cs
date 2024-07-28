using System.IO;
using System.Threading.Tasks;

namespace Proxel.Protocol.Helpers
{
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
