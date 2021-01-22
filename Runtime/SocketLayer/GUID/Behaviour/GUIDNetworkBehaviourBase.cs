using Assets.Models;
using Assets.SocketLayer.Client.Base;
using Assets.SocketLayer.PacketParser;
using Assets.SocketLayer.Serialization.Data;
using Bloodthirst.Socket;
using Bloodthirst.Socket.Core;
using Bloodthirst.Socket.Utils;
using System;

namespace Assets.SocketLayer.BehaviourComponent.NetworkPlayerEntity
{
    public abstract class GUIDNetworkBehaviourBase : NetworkBehaviourBase<Guid>, ISocketClient<GUIDSocketClient, Guid>, ISocketServer<Guid>
    {
        public ManagedSocketServer<Guid> SocketServer { get; set; }
        public GUIDSocketClient SocketClient { get; set; }

        private IPlayerSpawnSuccess[] spawnSuccess;

        private GUIDPlayerSpawnSuccessPacketServerProcessor GUIDPlayerSpawnSuccess;

        private void Awake()
        {
            if (!IsServer)
                return;

            spawnSuccess = GetComponentsInChildren<IPlayerSpawnSuccess>();

            GUIDPlayerSpawnSuccess = NetworkEntity.GetServer<GUIDPlayerSpawnSuccessPacketServerProcessor, GUIDPlayerSpawnSuccess>();

            GUIDPlayerSpawnSuccess.OnPacketParsedUnityThread += OnGUIDPlayerSpawnSuccess;
        }

        /// <summary>
        /// On recieving a message form the client confirming a successful spawn
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

        private void Start()
        {
            // if you are on client only
            // tehn send the server a message confirming that the player spawned successfully
            if (IsClient && !IsServer)
            {
                GUIDPlayerSpawnSuccess playerSpawnSuccess = new GUIDPlayerSpawnSuccess() { SpawnedPlayerID = NetworkID, ClientThePlayerSpawnedIn = SocketClient<Guid>.CurrentNetworkID };

                byte[] spawnPacket = PacketBuilder.BuildPacket(NetworkID, playerSpawnSuccess, SocketClient.IdentitySerializer, new GUIDPlayerSpawnSuccessNetworkSerializer());

                SocketClient.SendTCP(spawnPacket);
            }
        }
    }
}
