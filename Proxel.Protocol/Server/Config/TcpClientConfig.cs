using Proxel.Config;

namespace Proxel.Protocol.Server.Config
{
    internal class TcpClientConfig : Config<TcpClientConfig>
    {
        [Comment("The size of the receive buffer.")]
        public int ReceiveBufferSize { get; set; } = 8192;

        [Comment("The size of the send buffer.")]
        public int SendBufferSize { get; set; } = 8192;

        [Comment("The timeout for receiving data (in milliseconds).")]
        public int ReceiveTimeout { get; set; } = 2500;

        [Comment("The timeout for sending data (in milliseconds).")]
        public int SendTimeout { get; set; } = 2500;
    }
}