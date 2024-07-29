using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Proxel.Protocol.Enums;
using Proxel.Protocol.Structs;
using Proxel.Protocol.Helpers;
using Proxel.Protocol.Networking.Utils;

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
                Console.WriteLine($"Error handling TcpClient: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public static async Task HandlePacketAsync(NetworkStream stream)
        {
            Packet packet;
            try
            {
                packet = await PacketReader.ReadPacketAsync(stream);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client packet: {ex.Message}");
                return;
            }

            switch (packet.PacketId)
            {
                case 0: // Handshake (0x00)
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
            ushort protocolVersion = (ushort)await VarInt.ReadVarIntAsync(packetReader.BaseStream);
            string serverAddress = await FieldReader.ReadStringAsync(packetReader.BaseStream);
            ushort serverPort = FieldReader.ReadUnsignedShort(packetReader.BaseStream);
            ushort nextState = (ushort)await VarInt.ReadVarIntAsync(packetReader.BaseStream);
            Console.WriteLine($"HandleHandshakeAsync >> Protocol: {protocolVersion} Type: {nextState} Endpoint: {serverAddress}:{serverPort}");

            switch (nextState)
            {
                case 1: // Status
                    await HandleStatusRequestAsync(networkStream);
                    NetworkStreamDisposer.Dispose(networkStream);
                    break;
                case 2: // Login
                    await ProtocolCheck(protocolVersion, networkStream);
                    await HandleLoginRequestAsync(networkStream);
                    NetworkStreamDisposer.Dispose(networkStream);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported next state: {nextState}");
            }
        }

        private static async Task ProtocolCheck(ushort protocolVersion, NetworkStream networkStream)
        {
            List<ProtocolVersion> protocolVersions = ProtocolVersionUtils.GetProtocolVersion(protocolVersion);
            if (protocolVersions == null || protocolVersions.Count == 0)
            {
                using (var textBuilder = new TextBuilder()) // Build disconnect text
                {
                    textBuilder.Text = $"Unsupported version! ({protocolVersion})";
                    textBuilder.Bold = true;
                    Console.WriteLine($"JSON: {textBuilder.GetFinalJson()}");
                    using (var builder = new PacketBuilder(networkStream)) // Disconnect with reason
                    {
                        builder.SetPacketID(0x00);
                        builder.WriteString(textBuilder.GetFinalJson());
                        await builder.Send();
                    }
                }
            }
        }

        private static async Task HandleStatusRequestAsync(NetworkStream networkStream)
        {
            // Status Request from client
            ushort length = (ushort)await VarInt.ReadVarIntAsync(networkStream); // Should be 1
            ushort packetId = (ushort)await VarInt.ReadVarIntAsync(networkStream); // Should be 0
            if (length == 1 && packetId == 0)
            {
                Console.WriteLine($"HandleStatusRequestAsync >> Status Request detected!");
            }
            //else return;

            using (var packetBuilder = new PacketBuilder(networkStream))
            {
                packetBuilder.SetPacketID(0x00);
                using (var statusBuilder = new StatusBuilder())
                {
                    Console.WriteLine($"Status JSON: {statusBuilder.GetFinalJson()}");
                    packetBuilder.WriteByteArray(statusBuilder.GetFinalJsonAsByteArray());
                    await packetBuilder.Send();
                }
            }
        }

        private static async Task HandleLoginRequestAsync(NetworkStream networkStream)
        {
            Player player;
            Packet userDataPacket = await PacketReader.ReadPacketAsync(networkStream);
            using (BinaryReader reader = new(new MemoryStream(userDataPacket.Data)))
            {
                player = new(await FieldReader.ReadStringAsync(reader.BaseStream), await FieldReader.ReadUuidAsync(reader.BaseStream)); // Arg1 = userName | Arg2 = userUuid
            }
            Console.WriteLine($"HandleLoginRequestAsync >> User: {player.Name} UUID: {player.UUID}"); // TODO: Fix UUID parsing error in 1.12.2 (different packet scheme?)

            using (var builder = new PacketBuilder(networkStream)) // Login Success
            {
                builder.SetPacketID(0x02);
                builder.WriteUuid(player.UUID);
                builder.WriteString(player.Name);
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