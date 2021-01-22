using Bloodthirst.Socket.Core;
using Bloodthirst.Socket.PacketParser;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.SocketLayer.BehaviourComponent
{
    public class GUIDNetworkClientPlayerPacketProcessor : MonoBehaviour, IPacketClientProcessor<Guid>
    {

        private Dictionary<Guid, NetworkPlayerEntityBase<Guid>> playersActiveMap;

        [ShowInInspector]
        public IReadOnlyDictionary<Guid, NetworkPlayerEntityBase<Guid>> ReadbalePlayerActiveMap => playersActiveMap;

        private void Awake()
        {
            playersActiveMap = new Dictionary<Guid, NetworkPlayerEntityBase<Guid>>();
        }

        public NetworkPlayerEntityBase<Guid> RemoveNetworkEntityClient(Guid identifier)
        {
            NetworkPlayerEntityBase<Guid> player;

            if (!playersActiveMap.TryGetValue(identifier, out player))
                return null;

            playersActiveMap.Remove(identifier);

            return player;
        }

        public void AddNetworkEntityClient(Guid identifier, NetworkPlayerEntityBase<Guid> prefab)
        {
            playersActiveMap.Add(identifier, prefab);
        }

        public bool ProcessPacket(uint type, Guid from, byte[] data)
        {
            // if is not player packet leave

            if (from.Equals(GUIDIdentifier.DefaultClientID))
                return false;

            // if is from server

            if (GUIDIdentifier.IsServer(from))
                return false;

            // if player doesnt exist leave

            NetworkPlayerEntityBase<Guid> player;

            if (!playersActiveMap.TryGetValue(from, out player))
                return false;

            // execute the processor

            PacketClientProcessorBase<Guid> processor;

            if (!player.ReadableClientProcessorsMap.TryGetValue(type, out processor))
                return true;

            processor.Process(from, data);

            return true;
        }
    }
}
