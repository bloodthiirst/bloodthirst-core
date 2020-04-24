using Assets.Scripts.NetworkCommand;
using Bloodthirst.Socket.Core;
using Bloodthirst.Socket.Serializer;
using System;
using System.Net;
using System.Net.Sockets;

namespace Bloodthirst.Socket
{
    public class ConnectedClientSocket
    {
        public event Action<ConnectedClientSocket> OnDisconnect;

        public event Action<ConnectedClientSocket, byte[]> OnTCPMessage;

        public event Action<ConnectedClientSocket, byte[]> OnUDPMessage;

        private UdpClient UdpServer { get; set; }

        public TcpClient TcpClient  { get;set;}

        public IPEndPoint TCPClientIP { get; set; }

        public IPEndPoint UDPClientIP { get; set; }

        private byte[] Buffer = new byte[SocketConfig.Instance.PacketSize];

        private NetworkStream StreamTCP => TcpClient.GetStream();

        public void TriggerOnUDPMessage(byte[] data)
        {
            OnUDPMessage?.Invoke(this, data);
        }

        public ConnectedClientSocket(TcpClient tcpClient, UdpClient udpServer)
        {
            // init TCP

            TcpClient = tcpClient;

            UdpServer = udpServer;

            // init IPs

            TCPClientIP = (IPEndPoint)TcpClient.Client.RemoteEndPoint;

            UDPClientIP = new IPEndPoint(TCPClientIP.Address, TCPClientIP.Port + 1);

            StreamTCP.BeginRead(Buffer, 0, Buffer.Length, OnReadDone, null);
        }

        private void OnReadDone(IAsyncResult ar)
        {
            int Size = StreamTCP.EndRead(ar);

            if (Size <= 0)
            {
                // connecion is closed
                Close();
                return;
            }

            byte[] trimmedData = new byte[Size];

            Array.Copy(Buffer, trimmedData, Size);

            OnTCPMessage?.Invoke(this, trimmedData);

            StreamTCP.BeginRead(Buffer, 0, Buffer.Length, OnReadDone, null);
        }

        public void Close()
        {
            OnDisconnect?.Invoke(this);
        }

        /// <summary>
        /// Send to a update a specific player
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="from"></param>
        public void Send<TData, TKey>(TData t, TKey from, INetworkSerializer<TKey> identitySerializer , PROTOCOL protocol)
        {
            byte[] msg = NetworkUtils.GetCompressedData(t, from, identitySerializer);

            switch (protocol)
            {
                case PROTOCOL.TCP:
                    SendTCP(msg);
                    break;
                case PROTOCOL.UDP:
                    SendUDP(msg);
                    break;
                default:
                    break;
            }
            
        }

        /// <summary>
        /// Send to a update a specific player
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="from"></param>
        public void Send<TData ,TKey>(TData t , TKey from , INetworkSerializer<TData> networkSerializer, INetworkSerializer<TKey> identitySerializer, PROTOCOL protocol)
        {
            byte[] msg = NetworkUtils.GetCompressedData(t , from , networkSerializer , identitySerializer);

            switch (protocol)
            {
                case PROTOCOL.TCP:
                    SendTCP(msg);
                    break;
                case PROTOCOL.UDP:
                    SendUDP(msg);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Send raw data via TCP
        /// </summary>
        /// <param name="data"></param>
        public void SendTCP(byte[] data)
        {
            StreamTCP.Write(data, 0, data.Length );
        }


        /// <summary>
        /// Send raw data via UDP
        /// </summary>
        /// <param name="data"></param>
        public void SendUDP(byte[] data)
        {
            UdpServer.Send(data, data.Length, UDPClientIP);
        }

        ~ConnectedClientSocket()
        {
            TcpClient.Dispose();
            UdpServer.Dispose();
        }
    }
}
