using Bloodthirst.Socket;
using Bloodthirst.Socket.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class GUIDNetworkServerEntity : NetworkServerEntityBase<GUIDManagedSocketServer ,Guid>
{
    protected override GUIDManagedSocketServer CreateServer()
    {
        GUIDManagedSocketServer server = new GUIDManagedSocketServer(IPAddress.Parse(SocketConfig.Instance.ServerAddress), SocketConfig.Instance.WorldServerPort);

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
