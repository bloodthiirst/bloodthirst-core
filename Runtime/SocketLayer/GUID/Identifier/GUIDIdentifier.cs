using System;

namespace Bloodthirst.Socket.Core
{
    public class GUIDIdentifier : SocketIdentifier<GUIDIdentifier, Guid>
    {
        private Guid client;

        private Guid server;

        public GUIDIdentifier()
        {
            // start with the default empty guid

            byte[] serverArr = Guid.Empty.ToByteArray();

            byte[] clientArr = Guid.Empty.ToByteArray();

            // server has the last byte set to 1

            serverArr[15] = 1;

            // client has the last byte set to 0

            clientArr[15] = 0;

            client = new Guid(clientArr);

            server = new Guid(serverArr);
        }

        public override Guid DefaultClientIdentifier()
        {
            return client;
        }

        public override Guid DefaultServerIdentifier()
        {
            return server;
        }

        public override bool IsClientIdentifier(Guid identifier)
        {
            byte[] bytes = identifier.ToByteArray();

            return bytes[15] == 0;
        }

        public override bool IsServerIdentifier(Guid identifier)
        {
            byte[] bytes = identifier.ToByteArray();

            return bytes[15] == 1;
        }

        public override Guid GetClientIdentifier()
        {
            // start with default new guid

            byte[] clientArr = Guid.NewGuid().ToByteArray();

            // client has the last byte set to 0

            clientArr[15] = 0;

            Guid clientId = new Guid(clientArr);

            return clientId;
        }

        public override Guid GetServerIdentifier()
        {
            // start with default new guid

            byte[] serverArr = Guid.NewGuid().ToByteArray();

            // server has the last byte set to 1

            serverArr[15] = 1;

            Guid serverId = new Guid(serverArr);

            return serverId;
        }
    }
}
