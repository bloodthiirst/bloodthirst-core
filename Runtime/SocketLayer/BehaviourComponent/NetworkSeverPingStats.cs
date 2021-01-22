using Assets.Models;
using Assets.SocketLayer.PacketParser;
using Bloodthirst.Socket;
using Bloodthirst.Socket.Core;
using Bloodthirst.Socket.Serializer;
using Bloodthirst.Socket.Utils;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Assets.SocketLayer.BehaviourComponent
{
    public class NetworkSeverPingStats : MonoBehaviour, ISocketServer<Guid>
    {
        public ManagedSocketServer<Guid> SocketServer { get; set; }

        [SerializeField]
        private NetworkServerPinger networkPing;

        [SerializeField]
        private GUIDNetworkServerGlobalPacketProcessor packetProcessor;

        [ShowInInspector]
        private PingStatsRequestPacketServerProcessor PingStatsRequest { get; set; }

        private void Awake()
        {
            packetProcessor = GetComponent<GUIDNetworkServerGlobalPacketProcessor>();

            PingStatsRequest = packetProcessor.GetOrCreate<PingStatsRequestPacketServerProcessor, PingStatsRequest>();

            PingStatsRequest.OnPacketParsedThreaded += OnPingStatsRequest;
        }

        private void OnPingStatsRequest(PingStatsRequest req, Guid from, ConnectedClientSocket socket)
        {
            // TODO : FIX THIS => key not found when user connects

            if (!networkPing.NetworkPingStats.ContainsKey(socket))
            {
                return;
            }

            PingValues pingInfo = networkPing.NetworkPingStats[socket];

            PingStats stats = new PingStats()
            {
                PingTCP = (int)pingInfo.TCP,
                PingUDP = (int)pingInfo.UDP
            };

            byte[] packet = PacketBuilder.BuildPacket(GUIDIdentifier.DefaultClientID, stats, SocketServer.IdentifierSerializer, BaseNetworkSerializer<PingStats>.Instance);

            socket.SendTCP(packet);
        }
    }
}
