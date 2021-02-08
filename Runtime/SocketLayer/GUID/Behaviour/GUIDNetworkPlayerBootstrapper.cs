using Bloodthirst.Socket.Core;
using Bloodthirst.Socket.Models;
using Bloodthirst.Socket.PacketParser;
using Bloodthirst.Socket.Serialization;
using Bloodthirst.Socket.Serializer;
using Bloodthirst.Socket.Utils;
using System;

namespace Bloodthirst.Socket.BehaviourComponent.NetworkPlayerEntity
{
    /// <summary>
    /// <para> This component is responsible for announcing the spawn of the network entity</para>
    /// </summary>
    public class GUIDNetworkPlayerBootstrapper : GUIDNetworkBehaviourBase
    {
        private IPlayerSpawnSuccess[] spawnSuccess;

        private GUIDPlayerSpawnSuccessPacketServerProcessor GUIDPlayerSpawnSuccess;

        private INetworkSerializer<GUIDPlayerSpawnSuccess> playerSpawnSerializer;

        private void Awake()
        {
            playerSpawnSerializer = SerializerProvider.Get<GUIDPlayerSpawnSuccess>();

            if (!IsServer)
                return;

            spawnSuccess = GetComponentsInChildren<IPlayerSpawnSuccess>();

            GUIDPlayerSpawnSuccess = NetworkEntity.GetServer<GUIDPlayerSpawnSuccessPacketServerProcessor, GUIDPlayerSpawnSuccess>();

            GUIDPlayerSpawnSuccess.OnPacketParsedUnityThread += OnGUIDPlayerSpawnSuccess;
        }

        /// <summary>
        /// this callback gets triggered on recieving a message form the client confirming a successful spawn
        /// </summary>
        /// <param name="spawnInfo"></param>
        /// <param name="from"></param>
        /// <param name="socket"></param>
        private void OnGUIDPlayerSpawnSuccess(GUIDPlayerSpawnSuccess spawnInfo, Guid from, ConnectedClientSocket socket)
        {
            foreach (IPlayerSpawnSuccess spawn in spawnSuccess)
            {
                spawn.OnPlayerSpawnSuccess(spawnInfo);
            }
        }

        /// <summary>
        /// TODO : for the time being , this triggers for every behaviour that inherits from this , which means multiple times
        /// </summary>
        private void Start()
        {
            // if you are on client only
            // tehn send the server a message confirming that the player spawned successfully
            if (IsClient && !IsServer)
            {
                GUIDPlayerSpawnSuccess playerSpawnSuccess = new GUIDPlayerSpawnSuccess() { SpawnedPlayerID = NetworkID, ClientThePlayerSpawnedIn = SocketClient<Guid>.CurrentNetworkID };

                byte[] spawnPacket = PacketBuilder.BuildPacket(NetworkID, playerSpawnSuccess, SocketClient.IdentitySerializer, playerSpawnSerializer);

                SocketClient.SendTCP(spawnPacket);
            }
        }
    }
}
