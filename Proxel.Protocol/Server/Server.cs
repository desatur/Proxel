using System.IO;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using Proxel.Protocol.Types;

namespace Proxel.Protocol.Server
{
    public class Server
    {
        private readonly TcpListener _listener;

        public Server(IPAddress ipAddress = null, ushort port = 25565)
        {
            #if DEBUG
            ipAddress ??= IPAddress.Loopback;
            #else
            ipAddress ??= IPAddress.Any;
            #endif
            _listener = new TcpListener(ipAddress, port);
        }

        ~Server() 
        { 
            _listener.Stop();
            Console.WriteLine($"Server running on {_listener.LocalEndpoint} has stopped");
        }

        public async Task StartAsync()
        {
            _listener.Start();
            Console.WriteLine($"Server started on {_listener.LocalEndpoint}");

            while (true)
            {
                TcpClient client = await _listener.AcceptTcpClientAsync();
                _ = HandleClientAsync(client);
            }
        }

        private static async Task HandleClientAsync(TcpClient client)
        {
            using (NetworkStream networkStream = client.GetStream())
            {
                try
                {
                    await HandlePacketAsync(networkStream);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error handling client: {ex.Message}");
                }
            }
        }

        public static async Task HandlePacketAsync(NetworkStream stream)
        {
            McPacket packet;
            try
            {
                packet = await McPacket.ReadPacketAsync(stream);
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
                    string hexString = BitConverter.ToString(packet.Data).Replace("-", " ");
                    Console.WriteLine(msg + "\n----- DATA -----\n" + hexString);
                    throw new NotSupportedException(msg);
            }
        }

        private static async Task HandleHandshakeAsync(McPacket packet, NetworkStream networkStream)
        {
            using (var stream = new MemoryStream(packet.Data))
            using (var reader = new BinaryReader(stream))
            {
                int protocolVersion = await VarInt.ReadVarIntAsync(reader.BaseStream);
                string serverAddress = await PacketString.ReadStringAsync(reader.BaseStream);
                ushort serverPort = reader.ReadUInt16();
                int nextState = await VarInt.ReadVarIntAsync(reader.BaseStream);
                Console.WriteLine($"HandleHandshakeAsync >> Protocol: {protocolVersion} Type: {nextState} Endpoint: {serverAddress}:{serverPort}");

                switch (nextState)
                {
                    case 1: // Status TODO: Reimplement status
                        await HandleStatusRequestAsync(networkStream);
                        break;
                    case 2: // Login
                        await HandleLoginRequestAsync(networkStream);
                        break;
                    default:
                        throw new NotSupportedException($"Unsupported next state: {nextState}");
                }
            }
        }

        private static async Task HandleLoginRequestAsync(NetworkStream networkStream)
        {
            string username = "";
            string uuid = "";
            try // TODO: Fix handling Login packet sent by client to server, ~~I have made a workaround for my account~~
            {
                // Read the entire string data
                string stringData = await PacketString.ReadStringAsync(networkStream);
                byte[] data = Encoding.UTF8.GetBytes(stringData);

                if (data.Length < 16)
                {
                    throw new InvalidOperationException("Received data is too short to contain a valid UUID.");
                }

                // Extract UUID (last 16 bytes of the data)
                byte[] uuidBytes = new byte[16];
                Array.Copy(data, data.Length - 16, uuidBytes, 0, 16);

                // Extract Username (remaining bytes before the UUID)
                byte[] usernameBytes = new byte[data.Length - 16];
                Array.Copy(data, 0, usernameBytes, 0, usernameBytes.Length);

                // Decode UUID and Username
                uuid = GuidConverter.ToString(new Guid(uuidBytes));
                username = Encoding.UTF8.GetString(usernameBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading username and UUID: {ex.Message}");
                return;
            }

            Console.WriteLine($"HandleLoginRequestAsync >> User: {username} UUID: {uuid}");

            var dc = new
            {
                text = "Disconnect reason!"
            };

            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                try
                {
                    byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(dc));
                    await McPacket.SendMcPacketDataToClientAsync(networkStream, 0x00, data);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending login success packet: {ex.Message}");
                }
            }
        }
        private static async Task HandleStatusRequestAsync(NetworkStream networkStream)
        {
            // Status Request from client
            int length = await VarInt.ReadVarIntAsync(networkStream);
            int packetId = await VarInt.ReadVarIntAsync(networkStream);
            if (length == 1 && packetId == 0)
            {
                Console.WriteLine($"HandleStatusRequestAsync >> Status Request detected!");
            }
            else return;

            // Status Response to client
            //string statusString = JsonConvert.SerializeObject(File.ReadAllText(@"D:\testMOTD.txt"));
            byte[] data = File.ReadAllBytes(@"D:\testMOTD.txt"); /*Encoding.UTF8.GetBytes(statusString);*/
            Console.WriteLine($"HandleStatusRequestAsync >> statusJson:\n{Encoding.UTF8.GetString(data)}\n--- END ---");
            await McPacket.SendMcPacketDataToClientAsync(networkStream, 0x00, data);
            Console.WriteLine($"HandleStatusRequestAsync >> Sent Status");

            try
            {
                // Ping?
                int pingPacketLength = await VarInt.ReadVarIntAsync(networkStream);
                int pingPacketId = await VarInt.ReadVarIntAsync(networkStream);
                if (pingPacketId == 0x01)
                {
                    byte[] payload = new byte[pingPacketLength - 1]; // subtract 1 for the packet ID byte
                    await networkStream.ReadAsync(payload, 0, payload.Length);
                    await McPacket.SendMcPacketDataToClientAsync(networkStream, 0x01, payload);
                    Console.WriteLine($"HandleStatusRequestAsync >> Sent Ping Response");
                }
                else
                {
                    Console.WriteLine($"HandleStatusRequestAsync >> Unexpected Packet ID: {pingPacketId}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            //Console.WriteLine($"HandleStatusRequestAsync >> Len: {ping.Length} PacketID: {ping.PacketId} (Ping)");
        }
    }
}