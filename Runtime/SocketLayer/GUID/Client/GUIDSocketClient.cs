using Assets.SocketLayer.Serialization.Data;
using Bloodthirst.Socket;
using Bloodthirst.Socket.Serializer;
using System;
using System.Net;

namespace Assets.SocketLayer.Client.Base
{
    public class GUIDSocketClient : SocketClient<Guid>
    {
        private readonly INetworkSerializer<Guid> identitySerializer;

        public GUIDSocketClient(IPAddress ServerAddress, int serverPort) : base(ServerAddress, serverPort)
        {
            identitySerializer = new IdentityGUIDNetworkSerializer();
        }

        public override INetworkSerializer<Guid> IdentitySerializer => identitySerializer;
    }
}
