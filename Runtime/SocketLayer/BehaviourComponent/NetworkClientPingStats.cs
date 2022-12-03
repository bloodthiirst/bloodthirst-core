using Bloodthirst.Models;
using Bloodthirst.Socket.Client.Base;
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
    public class NetworkClientPingStats : MonoBehaviour, ISocketClient<GUIDSocketClient, Guid>
    {
        [SerializeField]
        private GUIDNetworkClientGlobalPacketProcessor packetProcessor;

        private PingStatsPacketClientProcessor PingRequest { get; set; }

        #if ODIN_INSPECTOR[ShowInInspector]#endif
        public GUIDSocketClient SocketClient { get; set; }

        private INetworkSerializer<PingStatsRequest> pingStatsRequestSerializer;

        #if ODIN_INSPECTOR[ShowInInspector]#endif
        #if ODIN_INSPECTOR[ReadOnly]#endif
        private int pingTCP;

        #if ODIN_INSPECTOR[ShowInInspector]#endif
        #if ODIN_INSPECTOR[ReadOnly]#endif
        private int pingUDP;

        [SerializeField]
        private float pingTimer;

        private float currentTimer;

        private void Awake()
        {
            pingStatsRequestSerializer = SerializerProvider.Get<PingStatsRequest>();

            packetProcessor = GetComponent<GUIDNetworkClientGlobalPacketProcessor>();

            PingRequest = packetProcessor.GetOrCreate<PingStatsPacketClientProcessor, PingStats>();

            PingRequest.OnPacketParsedThreaded -= OnPingStats;
            PingRequest.OnPacketParsedThreaded += OnPingStats;
        }

        private void OnDestroy()
        {
            PingRequest.OnPacketParsedThreaded -= OnPingStats;
        }

        private void Update()
        {
            if (SocketClient == null)
                return;

            if (!SocketClient.IsClient)
                return;

            currentTimer += Time.deltaTime;

            if (currentTimer < pingTimer)
            {
                return;
            }

            currentTimer = 0;

            byte[] pingReqPacket = PacketBuilder.BuildPacket(GUIDIdentifier.DefaultClientID, default, SocketClient.IdentitySerializer, pingStatsRequestSerializer);

            SocketClient.SendTCP(pingReqPacket);
        }

        private void OnPingStats(PingStats stats, Guid from)
        {
            pingTCP = stats.PingTCP;
            pingUDP = stats.PingUDP;
        }
    }
}
