using Bloodthirst.Socket;
using Bloodthirst.Socket.Core;
using System;
using System.Net;

public class GUIDGhostNetworkServerEntity : NetworkServerEntityBase<GUIDGhostManagedSocketServer, Guid>
{
    protected override GUIDGhostManagedSocketServer CreateServer()
    {
        GUIDGhostManagedSocketServer server = new GUIDGhostManagedSocketServer(IPAddress.Parse(SocketConfig.Instance.ServerAddress), SocketConfig.Instance.GhostServerPort);

        server.ServerConnexionManager.InjectServerTypes(QueryServerTypes());

        return server;
    }


    protected override void OnAnonymousConnected(ConnectedClientSocket anon)
    {
    }

    protected override void OnAnonymousMessage(ConnectedClientSocket from, byte[] data, PROTOCOL protocol)
    {
    }

    protected override void OnClientDisconnected(Guid id, ConnectedClientSocket connectedClient)
    {
    }

    protected override void OnManagedConnected(Guid id, ConnectedClientSocket connectedClient)
    {
    }

    protected override void OnManagedMessage(Guid id, ConnectedClientSocket from, byte[] data, PROTOCOL protocol)
    {
    }
}
