using Bloodthirst.Models;
using Bloodthirst.Socket.Serializer;
#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Bloodthirst.Socket.Core
{
    public abstract class ManagedSocketServer<TIdentifier> where TIdentifier : IComparable<TIdentifier>
    {
        public BasicSocketServer basicSocketServer;

        public BasicSocketServer BasicSocketServer => basicSocketServer;

        #if ODIN_INSPECTOR[ShowInInspector]#endif
        private ClientConnexionManager<TIdentifier> clientConnexionManager;
        public ClientConnexionManager<TIdentifier> ClientConnexionManager => clientConnexionManager;

        private ServerConnexionManager<TIdentifier> serverConnexionManager;

        public ServerConnexionManager<TIdentifier> ServerConnexionManager => serverConnexionManager;

        #if ODIN_INSPECTOR[ShowInInspector]#endif
        private List<ConnectedClientSocket> anonymousClients;

        public List<ConnectedClientSocket> AnonymousClients => anonymousClients;

        public int AnonymousClientsCount => AnonymousClients == null ? 0 : AnonymousClients.Count;

        public int ManagedClientsCount => ClientConnexionManager.ClientConnexions.Count;

        public event Action<ManagedSocketServer<TIdentifier>> OnServerStarted;

        /// <summary>
        /// On anonymous client connected
        /// </summary>
        public event Action<ConnectedClientSocket> OnAnonymousClientConnected;

        /// <summary>
        /// On anonymous client connected
        /// </summary>
        public event Action<ConnectedClientSocket> OnServerClientConnected;

        /// <summary>
        /// On managed client connection
        /// </summary>
        public event Action<TIdentifier, ConnectedClientSocket> OnManagedClientConnected;

        /// <summary>
        /// On message received from anonymous client
        /// </summary>
        public event Action<ConnectedClientSocket, byte[], PROTOCOL> OnAnonymousMessage;

        /// <summary>
        /// On message received from server client
        /// </summary>
        public event Action<ConnectedClientSocket, byte[], PROTOCOL> OnServerMessage;

        /// <summary>
        /// On message received from a managed client
        /// </summary>
        public event Action<TIdentifier, ConnectedClientSocket, byte[], PROTOCOL> OnManagedClientMessage;


        /// <summary>
        /// Event triggered when a client gets disconnected , this event works for both anonymous and maanged clients 
        /// </summary>
        public event Action<TIdentifier, ConnectedClientSocket> OnClientDisconnected;

        /// <summary>
        /// Event triggered when a client gets disconnected , this event works for both anonymous and maanged clients 
        /// </summary>
        public event Action<ConnectedClientSocket> OnServerClientDisconnected;

        public abstract INetworkSerializer<TIdentifier> IdentifierSerializer { get; }

        /// <summary>
        /// Get the list of managed connected clients
        /// </summary>
        /// <returns></returns>
        public List<TIdentifier> GetManagedClients()
        {
            return ClientConnexionManager.ClientConnexions.Keys.ToList();
        }


        public ManagedSocketServer(IPAddress IP, int Port)
        {
            basicSocketServer = new BasicSocketServer(IP, Port);

            clientConnexionManager = new ClientConnexionManager<TIdentifier>();

            serverConnexionManager = new ServerConnexionManager<TIdentifier>();

            anonymousClients = new List<ConnectedClientSocket>();

        }

        public ManagedSocketServer(IPAddress IP, int Port, IEqualityComparer<TIdentifier> equalityComparer)
        {
            basicSocketServer = new BasicSocketServer(IP, Port);

            clientConnexionManager = new ClientConnexionManager<TIdentifier>(equalityComparer);

            serverConnexionManager = new ServerConnexionManager<TIdentifier>();

            anonymousClients = new List<ConnectedClientSocket>();
        }

        public void Start()
        {
            basicSocketServer.OnClientConnected -= OnClientConnectedTriggered;
            basicSocketServer.OnClientConnected += OnClientConnectedTriggered;

            basicSocketServer.OnUnreliableData -= OnGlobalMessageUDPTriggered;
            basicSocketServer.OnUnreliableData += OnGlobalMessageUDPTriggered;


            basicSocketServer.Start();

            SocketConfig.Instance.IsServer = true;

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

            SocketConfig.Instance.IsServer = true;

        }

        public abstract TIdentifier GenerateClientIdentifier();

        public abstract TIdentifier GenerateServerIdentifier();

        public bool SetAsServerClient(ServerNetworkIDRequest request, ConnectedClientSocket connectedClient)
        {
            // if the hash of the server is not registed in the server types dictionary
            // then exit

            if (!serverConnexionManager.ContainsHash(request.ServerTypeHash))
            {
                return false;
            }

            // if client is not found in the anonymous list we exist

            if (!anonymousClients.Contains(connectedClient))
            {
                return false;
            }

            // else

            // add the client to the managed list

            serverConnexionManager.Add(request.ServerTypeHash, connectedClient);

            // remove the client form the anonymous list

            anonymousClients.Remove(connectedClient);

            // we unsubscribe from the anoymous message receiver 

            connectedClient.OnTCPMessage -= OnAnonymousMessageTCPTriggered;

            // and we start using the message receiver with the identifier

            connectedClient.OnTCPMessage += (client, msg) => OnServerMessage?.Invoke(connectedClient, msg, PROTOCOL.TCP);

            // we setup the udp messaging event

            // we unsubscribe from the anoymous message receiver 

            connectedClient.OnUDPMessage -= OnAnonymousMessageUDPTriggered;

            // and we start using the message receiver with the identifier

            connectedClient.OnUDPMessage += (client, msg) => OnServerMessage?.Invoke(connectedClient, msg, PROTOCOL.UDP);

            // same thing with the on disconnect event

            connectedClient.OnDisconnect -= OnAnonymousClientDisconnectedTriggered;

            connectedClient.OnDisconnect += (client) =>
            {
                OnServerClientDisconnected?.Invoke(connectedClient);
                serverConnexionManager.Remove(request.ServerTypeHash);
            };

            // trigger event

            OnServerClientConnected?.Invoke(connectedClient);

            return true;
        }

        public bool SetAsManagedClient(ConnectedClientSocket connectedClient, out TIdentifier identifier)
        {
            // if client is not found in the anonymous list we exit

            if (!anonymousClients.Contains(connectedClient))
            {
                identifier = default;

                return false;
            }

            // remove the client form the anonymous list

            anonymousClients.Remove(connectedClient);

            // else
            // generate an identitfier

            TIdentifier id = GenerateClientIdentifier();

            // add the client to the managed list

            clientConnexionManager.Add(id, connectedClient);

            // we unsubscribe from the anoymous message receiver 

            connectedClient.OnTCPMessage -= OnAnonymousMessageTCPTriggered;

            // and we start using the message receiver with the identifier

            connectedClient.OnTCPMessage += (client, msg) => OnManagedClientMessage?.Invoke(id, client, msg, PROTOCOL.TCP);

            // we setup the udp messaging event

            // we unsubscribe from the anoymous message receiver 

            connectedClient.OnUDPMessage -= OnAnonymousMessageUDPTriggered;

            // and we start using the message receiver with the identifier

            connectedClient.OnUDPMessage += (client, msg) => OnManagedClientMessage?.Invoke(id, connectedClient, msg, PROTOCOL.UDP);

            // same thing with the on disconnect event

            connectedClient.OnDisconnect -= OnAnonymousClientDisconnectedTriggered;

            connectedClient.OnDisconnect += (client) =>
            {
                OnClientDisconnected?.Invoke(id, connectedClient);
                ClientConnexionManager.Remove(id);
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

        public void BroadcastTCP(byte[] data, Predicate<TIdentifier> filter)
        {
            foreach (var kv in ClientConnexionManager.ClientConnexions)
            {
                if (!filter(kv.Key))
                    continue;

                kv.Value.SendTCP(data);
            }
        }

        public void BroadcastTCP(byte[] data)
        {
            foreach (ConnectedClientSocket socket in ClientConnexionManager.ClientConnexions.Values)
            {
                socket.SendTCP(data);
            }
        }

        /// <summary>
        /// If filter is not true , then the socket client will be skipped
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filter"></param>
        public void BroadcastUDP(byte[] data, Predicate<TIdentifier> filter)
        {
            foreach (var kv in ClientConnexionManager.ClientConnexions)
            {
                if (!filter(kv.Key))
                    continue;

                kv.Value.SendUDP(data);
            }
        }

        public void BroadcastUDP(byte[] data)
        {
            foreach (ConnectedClientSocket socket in ClientConnexionManager.ClientConnexions.Values)
            {
                socket.SendUDP(data);
            }
        }

        private void OnAnonymousMessageTCPTriggered(ConnectedClientSocket sender, byte[] message)
        {
            OnAnonymousMessage?.Invoke(sender, message, PROTOCOL.TCP);
        }

        private void OnGlobalMessageUDPTriggered(IPEndPoint sender, byte[] message)
        {
            // check if message is from managed client

            foreach (ConnectedClientSocket managed in clientConnexionManager.ClientConnexions.Values)
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
