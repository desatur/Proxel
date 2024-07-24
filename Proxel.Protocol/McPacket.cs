using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using Proxel.Protocol.Types;

namespace Proxel.Protocol
{
    public class McPacket
    {
        public int Length { get; set; }
        public int PacketId { get; set; }
        public byte[] Data { get; set; }

        public McPacket(int length, int packetId, byte[] data)
        {
            Length = length;
            PacketId = packetId;
            Data = data;
        }

        public static async Task<McPacket> ReadPacketAsync(Stream stream)
        {
            // Read length and packetId using VarInt
            int length = await VarInt.ReadVarIntAsync(stream);
            int packetId = await VarInt.ReadVarIntAsync(stream);

            // Calculate the size of the data
            int dataSize = length - VarInt.GetVarIntSize(packetId);
            var data = new byte[dataSize];

            int bytesRead = 0;
            while (bytesRead < dataSize)
            {
                int read = await stream.ReadAsync(data, bytesRead, dataSize - bytesRead);
                if (read == 0)
                {
                    throw new IOException("Unexpected end of stream.");
                }
                bytesRead += read;
            }
            return new McPacket(length, packetId, data);
        }

        public static async Task<Stream> CreatePacketStream(McPacket packet)
        {
            var stream = new MemoryStream();

            byte[] lengthBuffer = new byte[0];
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

        public static async Task SendDataToClientAsync(NetworkStream networkStream, byte[] data)
        {
            try
            {
                await networkStream.WriteAsync(data, 0, data.Length);
                await networkStream.FlushAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending data to client: {ex.Message}");
            }
        }
        public static async Task SendMcPacketDataToClientAsync(NetworkStream networkStream, int packetId, byte[] data)
        {
            try
            {
                int length = VarInt.GetVarIntSize(data.Length) + VarInt.GetVarIntSize(packetId) + data.Length;
                await VarInt.WriteVarIntAsync(networkStream, length);
                await VarInt.WriteVarIntAsync(networkStream, packetId);
                await networkStream.WriteAsync(data, 0, data.Length);
                await networkStream.FlushAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending data to client: {ex.Message}");
            }
        }

    }
}
