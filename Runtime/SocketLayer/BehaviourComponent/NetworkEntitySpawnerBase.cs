using Bloodthirst.Socket.Core;
using System;
using UnityEngine;

namespace Bloodthirst.Socket.BehaviourComponent
{
    public abstract class NetworkEntitySpawnerBase<TIdentity> : MonoBehaviour where TIdentity : IComparable<TIdentity>
    {
        protected SocketClient<TIdentity> SocketClient;

        protected ManagedSocketServer<TIdentity> SocketServer;


    }
}
