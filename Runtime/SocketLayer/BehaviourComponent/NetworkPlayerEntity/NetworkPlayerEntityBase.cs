using Bloodthirst.Socket.PacketParser;
using Bloodthirst.Utils;
#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Socket.BehaviourComponent
{
    public abstract class NetworkPlayerEntityBase<TIdentifier> : MonoBehaviour where TIdentifier : IComparable<TIdentifier>
    {
        private static bool IsServer => SocketConfig.Instance.IsServer;

        private static bool IsClient => SocketConfig.Instance.IsClient;

        #if ODIN_INSPECTOR[ShowInInspector]#endif
        #if ODIN_INSPECTOR[ReadOnly]#endif
        public TIdentifier NetworkID { get; set; }

        public bool IsPlayer => SocketClient<TIdentifier>.CurrentNetworkID.Equals(NetworkID);

        #if ODIN_INSPECTOR[ShowInInspector]#endif
        public Dictionary<uint, PacketClientProcessorBase<TIdentifier>> clientProcessorsMap;

        #if ODIN_INSPECTOR[ShowInInspector]#endif
        public Dictionary<uint, PacketServerProcessorBase<TIdentifier>> serverProcessorsMap;

        public IReadOnlyDictionary<uint, PacketClientProcessorBase<TIdentifier>> ReadableClientProcessorsMap => clientProcessorsMap;

        public IReadOnlyDictionary<uint, PacketServerProcessorBase<TIdentifier>> ReadableServerProcessorsMap => serverProcessorsMap;

        private void Awake()
        {
            clientProcessorsMap = new Dictionary<uint, PacketClientProcessorBase<TIdentifier>>();

            serverProcessorsMap = new Dictionary<uint, PacketServerProcessorBase<TIdentifier>>();
        }

        /// <summary>
        /// Get the packet client processor needed from the processors collection
        /// </summary>
        /// <typeparam name="TParser"></typeparam>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public TParser GetClient<TParser, TData>() where TParser : PacketClientProcessor<TData, TIdentifier>, new()
        {
            uint id = HashUtils.StringToHash(typeof(TData).Name);

            if (!clientProcessorsMap.ContainsKey(id))
            {
                TParser service = new TParser();
                clientProcessorsMap.Add(id, service);

                return service;
            }

            return (TParser)clientProcessorsMap[id];
        }

        /// <summary>
        /// Get the packet client processor needed from the processors collection
        /// </summary>
        /// <typeparam name="TParser"></typeparam>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public TParser GetServer<TParser, TData>() where TParser : PacketServerProcessor<TData, TIdentifier>, new()
        {
            uint id = HashUtils.StringToHash(typeof(TData).Name);

            if (!serverProcessorsMap.ContainsKey(id))
            {
                TParser service = new TParser();
                serverProcessorsMap.Add(id, service);

                return service;
            }

            return (TParser)serverProcessorsMap[id];
        }
    }
}