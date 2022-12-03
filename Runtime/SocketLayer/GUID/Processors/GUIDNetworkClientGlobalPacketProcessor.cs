using Bloodthirst.Socket.Core;
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
    public class GUIDNetworkClientGlobalPacketProcessor : MonoBehaviour, IPacketClientProcessor<Guid>
    {

        private Dictionary<uint, PacketClientProcessorBase<Guid>> packetProcessor;

        private Dictionary<uint, PacketClientProcessorBase<Guid>> PacketProcessor
        {
            get
            {
                if (packetProcessor == null)
                {
                    packetProcessor = new Dictionary<uint, PacketClientProcessorBase<Guid>>();
                }

                return packetProcessor;
            }
        }

        #if ODIN_INSPECTOR[ShowInInspector]#endif
        public IReadOnlyDictionary<uint, PacketClientProcessorBase<Guid>> ReadbalePacketProcessor => packetProcessor;

        public TProcessor GetOrCreate<TProcessor, TData>() where TProcessor : PacketClientProcessor<TData, Guid>, new()
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

        public bool ProcessPacket(uint type, Guid from, byte[] data)
        {
            // if is not global packet leave

            if (!from.Equals(GUIDIdentifier.DefaultClientID))
                return false;

            // if type doesnt have a processor

            PacketClientProcessorBase<Guid> processor;

            if (!PacketProcessor.TryGetValue(type, out processor))
                return false;

            // execute the processor

            processor.Process(from, data);

            return true;
        }
    }
}
