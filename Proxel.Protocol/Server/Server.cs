using System.IO;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text;
using Proxel.Protocol.Helpers;
using Proxel.Protocol.Packets;
using Proxel.Protocol.Packets.Utils;

namespace Proxel.Protocol.Server
{
    public class Server : IAsyncDisposable
    {
        private readonly TcpListener _listener;
        public bool Initiated { get; private set; }

        public Server(IPAddress ipAddress = null, ushort port = 25565)
        {
            #if DEBUG
            ipAddress ??= IPAddress.Loopback;
            #else
            ipAddress ??= IPAddress.Any;
            #endif
            _listener = new TcpListener(ipAddress, port);
        }
        ~Server() { DisposeAsync().AsTask().Wait(); }

        public async Task Start()
        {
            _listener.Start();
            Initiated = true;
            Console.WriteLine($"Server started on {_listener.LocalEndpoint}");
            while (true)
            {
                TcpClient client = await _listener.AcceptTcpClientAsync();
                _ = HandleClientAsync(client);
            }
        }

        public async Task Stop()
        {
            _listener.Stop();
            Initiated = false;
            Console.WriteLine($"Server running on {_listener.LocalEndpoint} has stopped");
        }

        private static async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                await HandlePacketAsync(client.GetStream());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling TcpClient: {ex.Message}");
            }
        }

        public static async Task HandlePacketAsync(NetworkStream stream)
        {
            Packet packet;
            try
            {
                packet = await Packet.ReadPacketAsync(stream);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client packet: {ex.Message}");
                return;
            }

            switch (packet.PacketId)
            {
                case 0x00: // Handshake
                    await HandleHandshakeAsync(packet, stream);
                    break;
                default:
                    string msg = $"Unsupported packet ID: {packet.PacketId} (HEX: {packet.PacketId:X2})";
                    Console.WriteLine(msg);
                    throw new NotSupportedException(msg);
            }
        }

        private static async Task HandleHandshakeAsync(Packet packet, NetworkStream networkStream)
        {
            BinaryReader packetReader = new(new MemoryStream(packet.Data));
            int protocolVersion = await VarInt.ReadVarIntAsync(packetReader.BaseStream);
            string serverAddress = await FieldReader.ReadStringAsync(packetReader.BaseStream);
            ushort serverPort = FieldReader.ReadUnsignedShort(packetReader.BaseStream);
            int nextState = await VarInt.ReadVarIntAsync(packetReader.BaseStream);
            Console.WriteLine($"HandleHandshakeAsync >> Protocol: {protocolVersion} Type: {nextState} Endpoint: {serverAddress}:{serverPort}");

            switch (nextState)
            {
                case 1: // Status
                    await HandleStatusRequestAsync(networkStream);
                    break;
                case 2: // Login
                    await HandleLoginRequestAsync(networkStream);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported next state: {nextState}");
            }
        }

        private static async Task HandleStatusRequestAsync(NetworkStream networkStream)
        {
            // Status Request from client
            int length = await VarInt.ReadVarIntAsync(networkStream); // Should be 1
            int packetId = await VarInt.ReadVarIntAsync(networkStream); // Should be 0
            if (length == 1 && packetId == 0)
            {
                Console.WriteLine($"HandleStatusRequestAsync >> Status Request detected!");
            }
            else return;

            byte[] status = File.ReadAllBytes(@"D:\testMOTD.txt"); // TODO: Do un-hardcode this, but PLEASE DO NOT use strings :pray:

            Console.WriteLine($"HandleStatusRequestAsync >> statusJson:\n{Encoding.UTF8.GetString(status)}\n--- END ---");
            await PacketWriter.WriteStringPacketAsync(networkStream, 0x00, status);

            Console.WriteLine($"HandleStatusRequestAsync >> Sent Status");
        }

        private static async Task HandleLoginRequestAsync(NetworkStream networkStream)
        {
            string userName = "";
            string userUuid = "";
            byte[] sharedSecret = [];

            Packet userDataPacket = await Packet.ReadPacketAsync(networkStream);
            using (BinaryReader reader = new(new MemoryStream(userDataPacket.Data)))
            {
                userName = await FieldReader.ReadStringAsync(reader.BaseStream);
                userUuid = await FieldReader.ReadUuidAsync(reader.BaseStream);
            }
            Console.WriteLine($"HandleLoginRequestAsync >> User: {userName} UUID: {userUuid}"); // TODO: Fix UUID parsing error in 1.12.2 (different packet scheme?)

            using (var builder = new PacketBuilder(networkStream)) // Login Success
            {
                builder.SetPacketID(0x02);
                builder.WriteUuid(userUuid);
                builder.WriteString(userName);
                builder.WriteVarInt(2);
                await builder.Send();
            }
        }

        public async ValueTask DisposeAsync()
        {
            await Stop();
            GC.SuppressFinalize(this);
        }
    }
}