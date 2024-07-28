using System;
using System.Numerics;
using System.Text;
using System.Security.Cryptography;

namespace Proxel.Protocol.Helpers
{
    public static class McCryptography
    {
        public static string MinecraftShaDigest(string input)
        {
            byte[] hash = SHA1.HashData(Encoding.UTF8.GetBytes(input));
            Array.Reverse(hash); // Reverse the bytes since BigInteger uses little endian
            BigInteger b = new(hash);
            // very annoyingly, BigInteger in C# tries to be smart and puts in
            // a leading 0 when formatting as a hex number to allow roundtripping 
            // of negative numbers, thus we have to trim it off.
            if (b < 0)
            {
                // toss in a negative sign if the interpreted number is negative
                return "-" + (-b).ToString("x").TrimStart('0');
            }
            else
            {
                return b.ToString("x").TrimStart('0');
            }
        }
        public static (string, string) GenRSA1024()
        {
            throw new NotImplementedException();
        }
    }
}
