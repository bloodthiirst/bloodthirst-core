using Bloodthirst.Socket;
using Bloodthirst.Socket.Core;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.SocketLayer.BehaviourComponent.NetworkPlayerEntity
{
    public abstract class NetworkBehaviourBase<TIdentifier> : MonoBehaviour where TIdentifier : IComparable<TIdentifier>
    {
        [ShowInInspector]
        protected static bool IsServer => SocketConfig.Instance.IsServer;

        [ShowInInspector]
        protected static bool IsClient => SocketConfig.Instance.IsClient;

        protected static bool HasPlayer => !SocketClient<TIdentifier>.CurrentNetworkID.Equals(GUIDIdentifier.DefaultClientID);

        private NetworkPlayerEntityBase<TIdentifier> networkEntity;

        protected NetworkPlayerEntityBase<TIdentifier> NetworkEntity
        {
            get
            {
                if(networkEntity == null)
                {
                    networkEntity = GetComponent<NetworkPlayerEntityBase<TIdentifier>>();
                }

                return networkEntity;
            }
        }

        [ShowInInspector]
        protected TIdentifier NetworkID => NetworkEntity.NetworkID;

        [ShowInInspector]
        protected bool IsPlayer => NetworkEntity.IsPlayer;

    }
}
