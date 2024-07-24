using System;

namespace Proxel.Protocol
{
    public static class GuidConverter
    {
        public static string ToString(Guid guid)
        {
            return guid.ToString("D");
        }

        public static Guid ToGuid(byte[] bytes)
        {
            if (bytes.Length != 16)
                throw new ArgumentException("Byte array must be 16 bytes long.");

            return new Guid(bytes);
        }
    }
}
