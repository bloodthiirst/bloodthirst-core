using Bloodthirst.Models;
using Bloodthirst.Socket.Core;
using Bloodthirst.Socket.PacketParser;
using Bloodthirst.Socket.Serialization;
using Bloodthirst.Socket.Serializer;
using Bloodthirst.Socket.Utils;
#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using System;
using UnityEngine;

namespace Bloodthirst.Socket.BehaviourComponent
{
    public class NetworkSeverPingStats : MonoBehaviour, ISocketServer<Guid>
    {
        public ManagedSocketServer<Guid> SocketServer { get; set; }

        [SerializeField]
        private NetworkServerPinger networkPing;

        [SerializeField]
        private GUIDNetworkServerGlobalPacketProcessor packetProcessor;

        #if ODIN_INSPECTOR[ShowInInspector]#endif
        private PingStatsRequestPacketServerProcessor PingStatsRequest { get; set; }

        private INetworkSerializer<PingStats> pingStatsSerializer;

        private void Awake()
        {
            packetProcessor = GetComponent<GUIDNetworkServerGlobalPacketProcessor>();

            pingStatsSerializer = SerializerProvider.Get<PingStats>();

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

            byte[] packet = PacketBuilder.BuildPacket(GUIDIdentifier.DefaultClientID, stats, SocketServer.IdentifierSerializer, pingStatsSerializer);

            socket.SendTCP(packet);
        }
    }
}
