using Assets.SocketLayer.Client.Base;
using Bloodthirst.Socket.Core;
using System;
using System.Net;
using UnityEngine;

namespace Assets.Scripts.SocketLayer.BehaviourComponent
{
    public class GUIDNetworkClientEntity : NetworkClientEntityBase<GUIDSocketClient, Guid>
    {
        protected override GUIDSocketClient CreateClient()
        {
            IPAddress serverAddress = null;

            if (!IPAddress.TryParse(this.ServerAddress, out serverAddress))
            {
                Debug.LogError("Error while parsing the server ip ...");
                return null;
            }


            return new GUIDSocketClient(serverAddress, SocketConfig.Instance.WorldServerPort);
        }
    }
}
