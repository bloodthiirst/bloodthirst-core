using Bloodthirst.Socket.Core;
#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using System;
using UnityEngine;

namespace Bloodthirst.Socket.BehaviourComponent.NetworkPlayerEntity
{
    public abstract class NetworkBehaviourBase<TIdentifier> : MonoBehaviour where TIdentifier : IComparable<TIdentifier>
    {
        #if ODIN_INSPECTOR[ShowInInspector]#endif
        protected static bool IsServer => SocketConfig.Instance.IsServer;

        #if ODIN_INSPECTOR[ShowInInspector]#endif
        protected static bool IsClient => SocketConfig.Instance.IsClient;

        protected static bool HasPlayer => !SocketClient<TIdentifier>.CurrentNetworkID.Equals(GUIDIdentifier.DefaultClientID);

        private NetworkPlayerEntityBase<TIdentifier> networkEntity;

        protected NetworkPlayerEntityBase<TIdentifier> NetworkEntity
        {
            get
            {
                if (networkEntity == null)
                {
                    networkEntity = GetComponent<NetworkPlayerEntityBase<TIdentifier>>();
                }

                return networkEntity;
            }
        }

        #if ODIN_INSPECTOR[ShowInInspector]#endif
        protected TIdentifier NetworkID => NetworkEntity.NetworkID;

        #if ODIN_INSPECTOR[ShowInInspector]#endif
        protected bool IsPlayer => NetworkEntity.IsPlayer;

    }
}
