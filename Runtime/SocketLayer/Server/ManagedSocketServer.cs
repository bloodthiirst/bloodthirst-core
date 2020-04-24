using Assets.Scripts.NetworkCommand;
using Assets.SocketLayer.PacketParser.Base;
using Bloodthirst.Socket.Core;
using Bloodthirst.Socket.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

namespace Bloodthirst.Socket
{
    public abstract class ManagedSocketServer<TIdentifier> where TIdentifier : IComparable<TIdentifier>
    {
        public BasicSocketServer basicSocketServer;

        public BasicSocketServer BasicSocketServer => basicSocketServer;

        private ConnexionManager<TIdentifier> connexionManager;
        public ConnexionManager<TIdentifier> ConnexionManager => connexionManager;

        private List<ConnectedClientSocket> anonymousClients;

        public List<ConnectedClientSocket> AnonymousClients => anonymousClients;

        public int AnonymousClientsCount => AnonymousClients == null ? 0 : AnonymousClients.Count;

        public int ManagedClientsCount => ConnexionManager.ClientConnexions.Count;

        public event Action<ManagedSocketServer<TIdentifier>> OnServerStarted;

        /// <summary>
        /// On anonymous client connected
        /// </summary>
        public event Action<ConnectedClientSocket> OnAnonymousClientConnected;

        /// <summary>
        /// On managed client connection
        /// </summary>
        public event Action<TIdentifier, ConnectedClientSocket> OnManagedClientConnected;

        /// <summary>
        /// On message received from anonymous client
        /// </summary>
        public event Action<ConnectedClientSocket, byte[] , PROTOCOL> OnAnonymousMessage;

        /// <summary>
        /// On message received from a managed client
        /// </summary>
        public event Action<TIdentifier, ConnectedClientSocket, byte[] , PROTOCOL> OnManagedClientMessage;


        /// <summary>
        /// Event triggered when a client gets disconnected , this event works for both anonymous and maanged clients 
        /// </summary>
        public event Action<TIdentifier , ConnectedClientSocket> OnClientDisconnected;

        /// <summary>
        /// geenrate an Identitfier
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public abstract TIdentifier GenerateIdentifer();

        public abstract INetworkSerializer<TIdentifier> IdentifierSerializer { get; }

        /// <summary>
        /// Get the list of managed connected clients
        /// </summary>
        /// <returns></returns>
        public List<TIdentifier> GetManagedClients()
        {
            return ConnexionManager.ClientConnexions.Keys.ToList();
        }

        public ManagedSocketServer(IPAddress IP, int Port)
        {
            basicSocketServer = new BasicSocketServer(IP, Port);

            connexionManager = new ConnexionManager<TIdentifier>();

            anonymousClients = new List<ConnectedClientSocket>();
        }

        public ManagedSocketServer(IPAddress IP, int Port, IEqualityComparer<TIdentifier> equalityComparer)
        {
            basicSocketServer = new BasicSocketServer(IP, Port);

            connexionManager = new ConnexionManager<TIdentifier>(equalityComparer);

            anonymousClients = new List<ConnectedClientSocket>();
        }

        public void Start()
        {
            basicSocketServer.OnClientConnected -= OnClientConnectedTriggered;
            basicSocketServer.OnClientConnected += OnClientConnectedTriggered;

            basicSocketServer.OnUnreliableData -= OnGlobalMessageUDPTriggered;
            basicSocketServer.OnUnreliableData += OnGlobalMessageUDPTriggered;


            basicSocketServer.Start();

            OnServerStarted?.Invoke(this);
        }

        public void Stop()
        {
            basicSocketServer.Stop();

            OnAnonymousClientConnected = null;
            OnManagedClientConnected = null;
            OnAnonymousMessage = null;
            OnManagedClientMessage = null;
            OnClientDisconnected = null;

        }

        public bool SetAsManagedClient(ConnectedClientSocket connectedClient, out TIdentifier identifier)
        {
            // if client is not found in the anonymous list we exist

            if (!anonymousClients.Contains(connectedClient))
            {
                identifier = default;

                return false;
            }

            // else
            // generate an identitfier

            TIdentifier id = GenerateIdentifer();
            // add the client to the managed list

            connexionManager.Add(id, connectedClient);

            // remove the client form the anonymous list

            anonymousClients.Remove(connectedClient);

            // we unsubscribe from the anoymous message receiver 

            connectedClient.OnTCPMessage -= OnAnonymousMessageTCPTriggered;

            // and we start using the message receiver with the identifier

            connectedClient.OnTCPMessage += (client, msg) => OnManagedClientMessage?.Invoke(id, connectedClient, msg , PROTOCOL.TCP);

            // we setup the udp messaging event

            // we unsubscribe from the anoymous message receiver 

            connectedClient.OnUDPMessage -= OnAnonymousMessageUDPTriggered;

            // and we start using the message receiver with the identifier

            connectedClient.OnUDPMessage += (client, msg) => OnManagedClientMessage?.Invoke(id, connectedClient, msg , PROTOCOL.UDP);

            // same thing with the on disconnect event

            connectedClient.OnDisconnect -= OnAnonymousClientDisconnectedTriggered;

            connectedClient.OnDisconnect += (client) =>
            {
                OnClientDisconnected?.Invoke(id , connectedClient);
                ConnexionManager.Remove(id);
            };

            // trigger event

            OnManagedClientConnected?.Invoke(id, connectedClient);

            identifier = id;

            return true;

        }

        private void OnClientConnectedTriggered(ConnectedClientSocket connectedClient)
        {
            anonymousClients.Add(connectedClient);

            connectedClient.OnDisconnect -= OnAnonymousClientDisconnectedTriggered;
            connectedClient.OnDisconnect += OnAnonymousClientDisconnectedTriggered;

            connectedClient.OnUDPMessage -= OnAnonymousMessageUDPTriggered;
            connectedClient.OnUDPMessage += OnAnonymousMessageUDPTriggered;

            connectedClient.OnTCPMessage -= OnAnonymousMessageTCPTriggered;
            connectedClient.OnTCPMessage += OnAnonymousMessageTCPTriggered;

            OnAnonymousClientConnected?.Invoke(connectedClient);
        }

        private void OnAnonymousMessageUDPTriggered(ConnectedClientSocket connectedClient, byte[] data)
        {
            OnAnonymousMessage?.Invoke(connectedClient, data, PROTOCOL.UDP);
        }

        public void BroadcastTCP(byte[] data , Predicate<TIdentifier> filter)
        {
            foreach (var kv in ConnexionManager.ClientConnexions)
            {
                if (!filter(kv.Key))
                    continue;

                kv.Value.SendTCP(data);
            }
        }

        public void BroadcastTCP(byte[] data)
        {
            foreach (ConnectedClientSocket socket in ConnexionManager.ClientConnexions.Values)
            {
                socket.SendTCP(data);
            }
        }

        public void BroadcastUDP(byte[] data , Predicate<TIdentifier> filter)
        {
            foreach(var kv in ConnexionManager.ClientConnexions)
            {
                if (!filter(kv.Key))
                    continue;

                kv.Value.SendUDP(data);
            }
        }

        public void BroadcastUDP(byte[] data)
        {
            foreach (ConnectedClientSocket socket in ConnexionManager.ClientConnexions.Values)
            {
                socket.SendUDP(data);
            }
        }

        private void OnAnonymousMessageTCPTriggered(ConnectedClientSocket sender, byte[] message)
        {   
            OnAnonymousMessage?.Invoke(sender, message , PROTOCOL.TCP);
        }

        private void OnGlobalMessageUDPTriggered(IPEndPoint sender, byte[] message)
        {
            // check if message is from managed client

            foreach (ConnectedClientSocket managed in connexionManager.ClientConnexions.Values)
            {
                if (managed.UDPClientIP.Address.Equals(sender.Address) && managed.UDPClientIP.Port == sender.Port)
                {
                    managed.TriggerOnUDPMessage(message);
                    return;
                }
            }

            // check if message is from anon client

            foreach (ConnectedClientSocket anon in AnonymousClients)
            {
                if (anon.UDPClientIP.Address.Equals(sender.Address) && anon.UDPClientIP.Port == sender.Port)
                {
                    anon.TriggerOnUDPMessage(message);
                    return;
                }
            }
        }

        private void OnAnonymousClientDisconnectedTriggered(ConnectedClientSocket disconnectedClient)
        {
            anonymousClients.Remove(disconnectedClient);
        }
    }
}
