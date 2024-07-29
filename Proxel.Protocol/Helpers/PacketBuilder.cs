using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Proxel.Protocol.Helpers
{
    public class PacketBuilder : IDisposable
    {
        private readonly MemoryStream _memoryStream;
        private readonly NetworkStream _networkStream;
        private readonly BinaryWriter _writer;
        public int PacketId { get; private set; } = 0x00;

        public PacketBuilder(NetworkStream networkStream)
        {
            _memoryStream = new MemoryStream();
            _networkStream = networkStream;
            _writer = new BinaryWriter(_memoryStream);
        }

        public void SetPacketID(int packetId)
        {
            if (packetId < 0 || packetId > 127)
            {
                throw new ArgumentException("Packet ID must be between 0 and 127 inclusive", nameof(packetId));
            }
            PacketId = packetId;
        }

        public void WriteByte(byte value)
        {
            _writer.Write(value);
        }
        public void WriteByteArray(byte[] values)
        {
            _writer.Write(values);
        }
        public void WriteShort(short value)
        {
            _writer.Write(value);
        }
        public void WriteInt(int value)
        {
            _writer.Write(value);
        }
        public void WriteLong(long value)
        {
            _writer.Write(value);
        }
        public void WriteFloat(float value)
        {
            _writer.Write(value);
        }
        public void WriteDouble(double value)
        {
            _writer.Write(value);
        }
        public void WriteString(string value)
        {
            byte[] stringBytes = System.Text.Encoding.UTF8.GetBytes(value);
            WriteVarInt(stringBytes.Length); // Write length of string
            _writer.Write(stringBytes); // Write the string
        }
        public void WriteUuid(string uuid)
        {
            if (string.IsNullOrEmpty(uuid))
            {
                throw new ArgumentNullException(nameof(uuid));
            }
            string uuidClean = uuid.Replace("-", "").ToLowerInvariant();
            if (uuidClean.Length != 32)
            {
                throw new ArgumentException("Invalid UUID format.");
            }

            // Parse the UUID string into bytes
            byte[] uuidBytes = new byte[16];
            for (int i = 0; i < 16; i++)
            {
                uuidBytes[i] = Convert.ToByte(uuidClean.Substring(i * 2, 2), 16);
            }

            ulong mostSigBits = BitConverter.ToUInt64(uuidBytes, 0);
            ulong leastSigBits = BitConverter.ToUInt64(uuidBytes, 8);

            _writer.Write(BitConverter.GetBytes(mostSigBits)); // Write the most significant bits (8 bytes) to the stream
            _writer.Write(BitConverter.GetBytes(leastSigBits)); // Write the least significant bits (8 bytes) to the stream
        }
        public void WriteVarInt(int value)
        {
            while ((value & -128) != 0)
            {
                _writer.Write((byte)(value & 127 | 128));
                value = (int)((uint)value >> 7);
            }
            _writer.Write((byte)value);
        }
        public void WriteVarLong(long value)
        {
            while ((value & -128L) != 0L)
            {
                _writer.Write((byte)((int)value & 127 | 128));
                value = (long)((ulong)value >> 7);
            }
            _writer.Write((byte)value);
        }

        public byte[] GetPacketBytes()
        {
            static void WriteVarInt(BinaryWriter writer, int value)
            {
                while ((value & -128) != 0)
                {
                    writer.Write((byte)(value & 127 | 128));
                    value = (int)((uint)value >> 7);
                }
                writer.Write((byte)value);
            }

            byte[] packetData = _memoryStream.ToArray();
            int packetLength = packetData.Length + 1; //Calculate the length of the packet (+1 because Packet ID is 1 byte, we do not need to calculate length of Packet ID)

            using (var finalStream = new MemoryStream())
            using (var finalWriter = new BinaryWriter(finalStream))
            {
                WriteVarInt(finalWriter, packetLength); // Write the length of the packet
                finalWriter.Write((byte)PacketId); // Write the Packet ID
                finalWriter.Write(packetData); // Write the packet data
                return finalStream.ToArray(); // Return the full packet in byte array
            }
        }

        public async Task Send()
        {
            byte[] packetBytes = GetPacketBytes();
            await _networkStream.WriteAsync(packetBytes, 0, packetBytes.Length);
        }

        public void Dispose()
        {
            _writer?.Dispose();
            _memoryStream?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
