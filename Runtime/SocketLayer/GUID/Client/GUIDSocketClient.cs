using System;
using System.Net;

namespace Bloodthirst.Socket.Client.Base
{
    public class GUIDSocketClient : SocketClient<Guid>
    {
        public GUIDSocketClient(IPAddress ServerAddress, int serverPort) : base(ServerAddress, serverPort)
        {
        }
    }
}
