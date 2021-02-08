using Bloodthirst.Models;
using Bloodthirst.Socket.Client.Base;
using Bloodthirst.Socket.Core;
using Bloodthirst.Socket.PacketParser;
using Bloodthirst.Socket.Serialization;
using Bloodthirst.Socket.Serializer;
using Bloodthirst.Socket.Utils;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Bloodthirst.Socket.BehaviourComponent
{
    public class NetworkClientPingStats : MonoBehaviour, ISocketClient<GUIDSocketClient, Guid>
    {
        [SerializeField]
        private GUIDNetworkClientGlobalPacketProcessor packetProcessor;

        private PingStatsPacketClientProcessor PingRequest { get; set; }

        [ShowInInspector]
        public GUIDSocketClient SocketClient { get; set; }

        private INetworkSerializer<PingStatsRequest> pingStatsRequestSerializer;

        [ShowInInspector]
        [ReadOnly]
        private int pingTCP;

        [ShowInInspector]
        [ReadOnly]
        private int pingUDP;

        [SerializeField]
        private float pingTimer;

        private float currentTimer;

        private void Awake()
        {
            pingStatsRequestSerializer = SerializerProvider.Get<PingStatsRequest>();

            packetProcessor = GetComponent<GUIDNetworkClientGlobalPacketProcessor>();

            PingRequest = packetProcessor.GetOrCreate<PingStatsPacketClientProcessor, PingStats>();

            PingRequest.OnPacketParsedThreaded += OnPingStats;
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
