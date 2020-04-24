using UnityEngine;
using System;
using Bloodthirst.Socket.PacketParser;
using System.Collections.Generic;
using Assets.SocketLayer.PacketParser;
using Assets.Scripts;
using Bloodthirst.Socket;
using Sirenix.OdinInspector;

namespace Assets.SocketLayer.BehaviourComponent
{
    public abstract class NetworkPlayerEntityBase<TIdentifier> : MonoBehaviour where TIdentifier : IComparable<TIdentifier>
    {
        private static bool IsServer => BasicSocketServer.IsServer;

        private static bool IsClient => SocketClient<Guid>.IsClient;

        [ShowInInspector]
        [ReadOnly]
        public TIdentifier NetworkID { get; set; }

        public bool IsPlayer => SocketClient<TIdentifier>.CurrentNetworkID.Equals(NetworkID);

        [ShowInInspector]
        public Dictionary<uint, PacketClientProcessorBase<TIdentifier>> clientProcessorsMap;

        [ShowInInspector]
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
        public TParser GetClient<TParser, TData>() where TParser : PacketClientProcessor<TData, TIdentifier> , new()
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