using Assets.Scripts;
using Assets.SocketLayer.PacketParser;
using Bloodthirst.Socket;
using Bloodthirst.Socket.Core;
using Bloodthirst.Socket.PacketParser;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.SocketLayer.BehaviourComponent
{
    public class GUIDNetworkClientServerMessagingPacketProcessor : MonoBehaviour, IPacketServerProcessor<Guid>
    {

        private Dictionary<uint, PacketServerProcessorBase<Guid>> packetProcessor;

        private Dictionary<uint, PacketServerProcessorBase<Guid>> PacketProcessor
        {
            get
            {
                if (packetProcessor == null)
                {
                    packetProcessor = new Dictionary<uint, PacketServerProcessorBase<Guid>>();
                }

                return packetProcessor;
            }
        }

        [ShowInInspector]
        public IReadOnlyDictionary<uint, PacketServerProcessorBase<Guid>> ReadbalePacketProcessor => packetProcessor;

        public TProcessor GetOrCreate<TProcessor, TData>() where TProcessor : PacketServerProcessor<TData, Guid>, new()
        {
            uint id = HashUtils.StringToHash(typeof(TData).Name);

            if (!PacketProcessor.ContainsKey(id))
            {
                TProcessor service = new TProcessor();
                PacketProcessor.Add(id, service);

                return service;
            }

            return (TProcessor)PacketProcessor[id];
        }

        public bool ProcessPacket(uint type, ConnectedClientSocket connectedClient, Guid from, byte[] data)
        {



            // if is not global packet leave

            if (!GUIDIdentifier.IsServer(from))
                return false;

            // if type doesnt have a processor

            if (!PacketProcessor.ContainsKey(type))
                return false;

            // execute the processor

            PacketProcessor[type].Process(connectedClient, from, data);

            return true;
        }
    }
}
