using Bloodthirst.Models;
using Bloodthirst.Socket;
using Bloodthirst.Socket.BehaviourComponent;
using Bloodthirst.Socket.Core;
using Bloodthirst.Socket.PacketParser;
using Bloodthirst.Socket.Serialization;
using Bloodthirst.Socket.Serializer;
using Bloodthirst.Socket.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PingValues
{
    [ShowInInspector]
    public long TCP { get; set; }

    [ShowInInspector]
    public long UDP { get; set; }

}

[RequireComponent(typeof(NetworkServerPinger))]
public class NetworkServerPinger : MonoBehaviour
{
    [SerializeField]
    private GUIDNetworkServerEntity networkServer;

    [SerializeField]
    private NetworkServerTimer networkTimer;

    [ShowInInspector]
    private Dictionary<ConnectedClientSocket, PingValues> networkPingStats;

    public IReadOnlyDictionary<ConnectedClientSocket, PingValues> NetworkPingStats => networkPingStats;

    [SerializeField]
    private GUIDNetworkServerGlobalPacketProcessor packetProcessor;

    [ShowInInspector]
    private PingPongUDPPacketServerProcessor pingPongUDPParser;

    [ShowInInspector]
    private PingPongTCPPacketServerProcessor pingPongTCPParser;

    [SerializeField]
    private float pingTimer;

    private float currentTimer;

    private INetworkSerializer<Guid> guidSerializer;

    private INetworkSerializer<PingPongTCP> pingPongTCPSerializer;

    private INetworkSerializer<PingPongUDP> pingPongUDPSerializer;

    private void Awake()
    {
        pingPongTCPSerializer = SerializerProvider.Get<PingPongTCP>();
        pingPongUDPSerializer = SerializerProvider.Get<PingPongUDP>();
        guidSerializer = SerializerProvider.Get<Guid>();

        networkPingStats = new Dictionary<ConnectedClientSocket, PingValues>();

        if (networkServer == null)
        {
            networkServer = GetComponent<GUIDNetworkServerEntity>();
        }

        if (networkTimer == null)
        {
            networkTimer = GetComponent<NetworkServerTimer>();
        }

        if (packetProcessor == null)
        {
            packetProcessor = GetComponent<GUIDNetworkServerGlobalPacketProcessor>();
        }

        pingPongUDPParser = packetProcessor.GetOrCreate<PingPongUDPPacketServerProcessor, PingPongUDP>();

        pingPongUDPParser.OnPacketParsedThreaded -= PingUDPReceived;
        pingPongUDPParser.OnPacketParsedThreaded += PingUDPReceived;

        pingPongTCPParser = packetProcessor.GetOrCreate<PingPongTCPPacketServerProcessor, PingPongTCP>();

        pingPongTCPParser.OnPacketParsedThreaded -= PingTCPReceived;
        pingPongTCPParser.OnPacketParsedThreaded += PingTCPReceived;

    }

    private void PingTCPReceived(PingPongTCP ping, Guid from, ConnectedClientSocket connectedClient)
    {
        long pingResult = networkTimer.TimeElapsed - ping.SentAtServerTime;

        networkPingStats[connectedClient].TCP = pingResult;
    }

    private void PingUDPReceived(PingPongUDP ping, Guid from, ConnectedClientSocket connectedClient)
    {
        long pingResult = networkTimer.TimeElapsed - ping.SentAtServerTime;

        networkPingStats[connectedClient].UDP = pingResult;

    }

    private void OnValidate()
    {
        networkServer = GetComponent<GUIDNetworkServerEntity>();

        networkTimer = GetComponent<NetworkServerTimer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GUIDNetworkServerEntity.IsServer)
            return;

        currentTimer += Time.deltaTime;

        if (currentTimer < pingTimer)
        {
            return;
        }

        currentTimer = 0;

        // iterate over the connected clients

        foreach (ConnectedClientSocket client in networkServer.SocketServer.AnonymousClients)
        {
            // register the clients if they dont exist
            // UDP

            if (!networkPingStats.ContainsKey(client))
            {
                networkPingStats.Add(client, new PingValues());
            }


            // create the ping packet and send it
            // UDP

            PingPongUDP pingUDP = new PingPongUDP() { SentAtServerTime = networkTimer.TimeElapsed };

            byte[] pingUDPPacket = PacketBuilder.BuildPacket(GUIDIdentifier.DefaultClientID, pingUDP, guidSerializer, pingPongUDPSerializer);

            client.SendUDP(pingUDPPacket);

            // TCP

            PingPongTCP pingTCP = new PingPongTCP() { SentAtServerTime = networkTimer.TimeElapsed };

            byte[] pingTCPPacket = PacketBuilder.BuildPacket(GUIDIdentifier.DefaultClientID, pingTCP, guidSerializer, pingPongTCPSerializer);

            client.SendTCP(pingTCPPacket);
        }

    }
}
