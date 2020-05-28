using Assets.Models;
using Assets.SocketLayer.PacketParser;
using Assets.SocketLayer.PacketParser.Base;
using Bloodthirst.Socket;
using Bloodthirst.Socket.Core;
using Bloodthirst.Socket.Serializer;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.SocketLayer.BehaviourComponent
{
    public class NetworkClientPingStats : MonoBehaviour, ISocketClient<Guid>
    {
        [SerializeField]
        private GUIDNetworkClientGlobalPacketProcessor packetProcessor;

        private PingStatsPacketClientProcessor PingRequest { get; set; }

        [ShowInInspector]
        public SocketClient<Guid> SocketClient { get; set; }

        [ShowInInspector]
        [ReadOnly]
        private int pingTCP;

        [ShowInInspector]
        [ReadOnly]
        private int pingUDP;


        [SerializeField]
        private float pingTimer = 0;

        private float currentTimer;

        private void Awake()
        {
            packetProcessor = GetComponent<GUIDNetworkClientGlobalPacketProcessor>();

            PingRequest = packetProcessor.GetOrCreate<PingStatsPacketClientProcessor, PingStats>();

            PingRequest.OnPacketParsedThreaded += OnPingStats;
        }

        private void Update()
        {
            if (!SocketClient<Guid>.IsClient)
                return;

            currentTimer += Time.deltaTime;

            if (currentTimer < pingTimer)
            {
                return;
            }

            currentTimer = 0;

            byte[] pingReqPacket = PacketBuilder.BuildPacket(SocketIdentifier<Guid>.Get, default , SocketClient.IdentitySerializer, BaseNetworkSerializer<PingStatsRequest>.Instance);

            SocketClient.SendTCP(pingReqPacket);
        }

        private void OnPingStats(PingStats stats, Guid from)
        {
            pingTCP = stats.PingTCP;
            pingUDP = stats.PingUDP;
        }
    }
}
