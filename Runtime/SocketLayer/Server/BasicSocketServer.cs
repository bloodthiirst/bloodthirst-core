using System;
using System.Net;
using System.Net.Sockets;

namespace Bloodthirst.Socket
{
    public class BasicSocketServer
    {
        private bool isServer = false;

        public bool IsServer => isServer;

        private IPAddress ip;

        public IPAddress IP => ip;

        private int port;

        public int Port => port;

        private TcpListener TcpListener { get; set; }

        public UdpClient UdpServer { get; set; }

        public event Action<ConnectedClientSocket> OnClientConnected;

        public event Action<IPEndPoint,byte[]> OnUnreliableData;




        public BasicSocketServer(IPAddress ip , int port)
        {
            this.ip = ip;
            this.port = port;

            // tcp

            IPEndPoint tcpEndpoint = new IPEndPoint(ip,port);

            TcpListener = new TcpListener(tcpEndpoint);
            
            // udp

            IPEndPoint udpEndpoint = new IPEndPoint(ip, port + 1);

            UdpServer = new UdpClient(udpEndpoint);
        }

        public void Start()
        {
            TcpListener.Start();

            TcpListener.BeginAcceptTcpClient(OnClientConnectedCallback, null);

            UdpServer.BeginReceive(OnUDPReceive, null);

            isServer = true;
        }

        private void OnUDPReceive(IAsyncResult ar)
        {
            IPEndPoint ip = null;

            byte[] data = UdpServer.EndReceive(ar ,ref ip);

            OnUnreliableData?.Invoke(ip, data);

            UdpServer.BeginReceive(OnUDPReceive, null);

        }

        public void Stop()
        {
            TcpListener?.Stop();

            UdpServer?.Close();

            isServer = false;
        }

        private void OnClientConnectedCallback(IAsyncResult ar)
        {
            // end accept client attempt and get the client
            TcpClient tcpClient = TcpListener.EndAcceptTcpClient(ar);

            tcpClient.NoDelay = true;

            ConnectedClientSocket connectedClient = new ConnectedClientSocket(tcpClient , UdpServer);

            OnClientConnected?.Invoke(connectedClient);

            // retart waiting for the next client to join

            TcpListener.BeginAcceptTcpClient(OnClientConnectedCallback, null);
        }
    }
}
