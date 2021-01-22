using Bloodthirst.Socket.Core;
using Bloodthirst.Socket.Serializer;
using Sirenix.OdinInspector;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Bloodthirst.Socket
{
    public abstract class SocketClient<TIdentifier> where TIdentifier : IComparable<TIdentifier>
    {
        [BoxGroup]
        [ShowInInspector]
        public static TIdentifier CurrentNetworkID;

        [ShowInInspector]
        public bool IsClient => isClient;

        private bool isClient = false;

        public int PortTCP;

        public int PortUDP;

        public event Action OnConnect;

        public event Action<SocketClient<TIdentifier>> OnDisconnect;

        public event Action<SocketClient<TIdentifier>, byte[], PROTOCOL> OnMessage;

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
                    udpServerEndpoint = new IPEndPoint(serverAddress, serverPort + 10);
                }
                return udpServerEndpoint;
            }
        }

        #endregion


        private int serverPort;

        public int Port => serverPort;

        public TcpClient TcpClient { get; set; }

        public UdpClient UdpClient { get; set; }

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

            OnMessage?.Invoke(this, data, PROTOCOL.UDP);

            UdpClient.BeginReceive(OnRevieveUDP, null);
        }

        private void OnConnected(IAsyncResult ar)
        {
            isClient = true;

            // stop the connecting attempt since its done and get the connection

            TcpClient.EndConnect(ar);

            PortTCP = ((IPEndPoint)TcpClient.Client.LocalEndPoint).Port;

            PortUDP = PortTCP + 10;

            Debug.Log("TCP CLient port : " + PortTCP);
            Debug.Log("UDP CLient port : " + PortUDP);


            // UDP
            IPEndPoint udpIP = new IPEndPoint(((IPEndPoint)TcpClient.Client.LocalEndPoint).Address, PortUDP);

            UdpClient = new UdpClient(udpIP);

            // start listening to server messages

            UdpClient.BeginReceive(OnRevieveUDP, null);

            TcpClient.GetStream().BeginRead(Buffer, 0, Buffer.Length, OnRead, null);

            // trigger on connect event

            OnConnect?.Invoke();
        }

        public void SendUDP(byte[] packet)
        {
            UdpClient.Send(packet, packet.Length, UDPServerEndpoint);
        }

        public void SendTCP(byte[] packet)
        {
            try
            {
                TcpClient.GetStream().Write(packet, 0, packet.Length);
            }
            catch (Exception ex)
            {
                Debug.Break();
                Debug.LogError(ex.Message);
            }
        }


        void OnRead(IAsyncResult ar)
        {
            // stop reading attempt and get message size*

            int Size = TcpClient.GetStream().EndRead(ar);

            if (Size <= 0)
            {
                isClient = false;

                // Connection closed

                Disconnect();

                return;
            }

            byte[] trimmedData = new byte[Size];

            Array.Copy(Buffer, trimmedData, Size);

            OnMessage?.Invoke(this, trimmedData, PROTOCOL.TCP);

            // restart reading attend for next message

            TcpClient.GetStream().BeginRead(Buffer, 0, Buffer.Length, OnRead, null);
        }

        public void Disconnect()
        {
            isClient = false;

            OnDisconnect?.Invoke(this);

            TcpClient.Close();
            UdpClient.Close();

            TcpClient.Dispose();
            UdpClient.Dispose();
        }
    }
}
