using Proxel.Log4Console;
using Proxel.Protocol.Helpers;
using Proxel.Protocol.Networking.Utils;
using Proxel.Protocol.Structs;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Proxel.Protocol.Server
{
    public class LoginHandler
    {
        internal static List<Player> Players {  get; private set; }
        internal static async Task HandleLoginRequestAsync(NetworkStream networkStream, PlayerConnectionInfo playerConnectionInfo)
        {
            Player player;
            Packet userDataPacket = await PacketReader.ReadPacketAsync(networkStream);
            using (BinaryReader reader = new(new MemoryStream(userDataPacket.Data))) // TODO: Fix UUID parsing error in 1.12.2 (different packet scheme?)
            {
                player = new(await FieldReader.ReadStringAsync(reader.BaseStream), await FieldReader.ReadUuidAsync(reader.BaseStream), playerConnectionInfo);
            }
            if (await ProtocolVersionUtils.DisconnectIfUnsupportedProtocol(networkStream, player, playerConnectionInfo)) return;
            Players.Add(player); // TODO: Move this after Login Success
            Log.Info("UUID of player " + player.Name + " is " + player.UUID, "User Authenticator");

            using (var builder = new PacketBuilder(networkStream)) // TODO: Fix Login Success
            {
                builder.SetPacketID(0x02);
                builder.WriteUuid(player.UUID);
                builder.WriteString(player.Name);
                builder.WriteVarInt(2);
                await builder.Send();
            }
        }
    }
}
