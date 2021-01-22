using Assets.SocketLayer.Client.Base;
using Bloodthirst.Socket;
using Bloodthirst.Socket.Core;
using System;
using System.Net;
using UnityEngine;

namespace Assets.Scripts.SocketLayer.BehaviourComponent
{
    public class GUIDGhostNetworkClientEntity : NetworkClientEntityBase<GUIDGhostSocketClient, Guid>, IServerToServerConnection<Guid>
    {
        public Tuple<Type, SocketClient<Guid>> SocketToServer()
        {
            return new Tuple<Type, SocketClient<Guid>>(typeof(GUIDGhostManagedSocketServer), SocketClient);
        }

        protected override GUIDGhostSocketClient CreateClient()
        {
            IPAddress serverAddress = null;

            if (!IPAddress.TryParse(this.ServerAddress, out serverAddress))
            {
                Debug.LogError("Error while parsing the server ip ...");
                return null;
            }


            return new GUIDGhostSocketClient(serverAddress, SocketConfig.Instance.GhostServerPort);
        }
    }
}
