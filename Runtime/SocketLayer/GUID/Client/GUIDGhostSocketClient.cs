using System;
using System.Net;

namespace Bloodthirst.Socket.Client.Base
{
    public class GUIDGhostSocketClient : SocketClient<Guid>
    {
        public GUIDGhostSocketClient(IPAddress ServerAddress, int serverPort) : base(ServerAddress, serverPort)
        {
        }
    }
}
