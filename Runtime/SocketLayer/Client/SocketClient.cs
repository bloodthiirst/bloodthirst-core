using Assets.Scripts.NetworkCommand;
using Assets.SocketLayer.Serialization.Data;
using Bloodthirst.Socket.Core;
using Bloodthirst.Socket.Serializer;
using Sirenix.OdinInspector;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Bloodthirst.Socket
{
    public abstract class SocketClient<TIdentifier>  where TIdentifier : IComparable<TIdentifier>
    {
        [BoxGroup]
        [ShowInInspector]
        public static TIdentifier CurrentNetworkID;

        [ShowInInspector]
        public static bool IsClient => isClient;

        private static bool isClient = false;

        public int PortTCP;

        public int PortUDP;

        public event Action OnConnect;
        
        public event Action<SocketClient<TIdentifier>> OnDisconnect;

        public event Action<SocketClient<TIdentifier>, byte[] , PROTOCOL> OnMessage;

        private IPAddress serverAddress;

        public IPAddress ServerAddress => serverAddress;

        #region TCP address

        private IPEndPoint serverEndpoint;

        private IPEndPoint ServerEndpoint
        {
            get
            {
                if (serverEndpoint == null)
                {
                    serverEndpoint = new IPEndPoint(serverAddress, serverPort);
                }
                return serverEndpoint;
            }
        }

        #endregion

        #region UDP address

        private IPEndPoint udpServerEndpoint;

        private IPEndPoint UDPServerEndpoint
        {
            get
            {
                if (udpServerEndpoint == null)
                {
                    udpServerEndpoint = new IPEndPoint(serverAddress, serverPort + 1);
                }
                return udpServerEndpoint;
            }
        }
        
        #endregion


        private int serverPort;

        public int Port => serverPort;

        public TcpClient TcpClient { get; set; }

        public UdpClient UdpClient { get; set; }

        private NetworkStream Stream => TcpClient.GetStream();

        public abstract INetworkSerializer<TIdentifier> IdentitySerializer { get; }

        private byte[] Buffer = new byte[SocketConfig.Instance.PacketSize];

        public SocketClient(IPAddress ServerAddress, int serverPort)
        {
            this.serverAddress = ServerAddress;
            this.serverPort = serverPort;

            // TCP
            TcpClient = new TcpClient();
            TcpClient.NoDelay = true;
        }

        public void Connect()
        {
            TcpClient.BeginConnect(serverAddress, serverPort, OnConnected, null);
        }

        private void OnRevieveUDP(IAsyncResult ar)
        {
            IPEndPoint ip = null;

            byte[] data = UdpClient.EndReceive(ar, ref ip);

            OnMessage?.Invoke(this, data , PROTOCOL.UDP);

            UdpClient.BeginReceive(OnRevieveUDP, null);
        }

        private void OnConnected(IAsyncResult ar)
        {        
            isClient = true;

            // stop the connecting attempt since its done and get the connection

            TcpClient.EndConnect(ar);

            PortTCP = ((IPEndPoint)TcpClient.Client.LocalEndPoint).Port;

            PortUDP = PortTCP + 1;

            Debug.Log("TCP CLient port : " + PortTCP);
            Debug.Log("UDP CLient port : " + PortUDP);


            // UDP
            IPEndPoint udpIP = new IPEndPoint(((IPEndPoint)TcpClient.Client.LocalEndPoint).Address, PortUDP);

            UdpClient = new UdpClient(udpIP);

            // start listening to server messages

            UdpClient.BeginReceive(OnRevieveUDP, null);

            Stream.BeginRead(Buffer, 0, Buffer.Length, OnRead, null);

            // trigger on connect event

            OnConnect?.Invoke();
        }

        /// <summary>
        /// Send the data using a specific serializer
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="data"></param>
        /// <param name="from"></param>
        /// <param name="protocol"></param>
        /// <param name="serializer"></param>
        public void Send<TData>(TData data, TIdentifier from, PROTOCOL protocol , INetworkSerializer<TData> serializer)
        {
            byte[] package = NetworkUtils.GetCompressedData(data, from, serializer , IdentitySerializer);

            switch (protocol)
            {
                case PROTOCOL.TCP:
                    Stream.Write(package, 0, package.Length);
                    break;
                case PROTOCOL.UDP:
                    UdpClient.Send(package , package.Length, UDPServerEndpoint);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Send data using the BaseNEtworkSerializer
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="data"></param>
        /// <param name="from"></param>
        /// <param name="protocol"></param>
        public void Send<TData>(TData data , TIdentifier from , PROTOCOL protocol)
        {
            byte[] package = NetworkUtils.GetCompressedData(data , from , IdentitySerializer );

            switch (protocol)
            {
                case PROTOCOL.TCP:
                    Stream.Write(package, 0, package.Length);
                    break;
                case PROTOCOL.UDP:
                    UdpClient.Send(package, package.Length, UDPServerEndpoint);
                    break;
                default:
                    break;
            }
        }

        public void SendUDP(byte[] packet)
        {
            UdpClient.Send(packet, packet.Length, UDPServerEndpoint);
        }

        public void SendTCP(byte[] packet)
        {
            Stream.Write(packet, 0, packet.Length);
        }


        void OnRead(IAsyncResult ar)
        {
            // stop reading attempt and get message size*

            int Size = Stream.EndRead(ar);

            if (Size <= 0)
            {
                isClient = false;

                // Connection closed

                OnDisconnect?.Invoke(this);

                return;
            }

            byte[] trimmedData = new byte[Size];

            Array.Copy(Buffer, trimmedData, Size);

            OnMessage?.Invoke(this, trimmedData , PROTOCOL.TCP);

            // restart reading attend for next message

            Stream.BeginRead(Buffer, 0, Buffer.Length, OnRead, null);
        }

        public void Disconnect()
        {
            isClient = false;

            OnDisconnect?.Invoke(this);

            TcpClient.Close();
            UdpClient.Close();
        }

        ~SocketClient()
        {
            TcpClient.Dispose();
            UdpClient.Dispose();
        }
    }
}
