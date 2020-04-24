using Assets.SocketLayer.Client.Base;
using Bloodthirst.Socket;
using Bloodthirst.Socket.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.SocketLayer.BehaviourComponent
{
    public class GUIDNetworkClientEntity : NetworkClientEntityBase<Guid>
    {
        protected override SocketClient<Guid> CreateClient()
        {
            IPAddress serverAddress = null;

            if (!IPAddress.TryParse(this.ServerAddress, out serverAddress))
            {
                Debug.LogError("Error while parsing the server ip ...");
                return null;
            }


            return new GUIDSocketClient(serverAddress, ServerPort);
        }
    }
}
