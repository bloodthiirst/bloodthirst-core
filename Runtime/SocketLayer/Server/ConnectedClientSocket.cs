#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Bloodthirst.Socket
{
    public class ConnectedClientSocket
    {
        public event Action<ConnectedClientSocket> OnDisconnect;

        public event Action<ConnectedClientSocket, byte[]> OnTCPMessage;

        public event Action<ConnectedClientSocket, byte[]> OnUDPMessage;

        private UdpClient UDPServer { get; set; }

        public TcpClient TcpClient { get; set; }

        #if ODIN_INSPECTOR[ShowInInspector]#endif
        public IPEndPoint TCPClientIP { get; set; }

        #if ODIN_INSPECTOR[ShowInInspector]#endif
        public IPEndPoint UDPClientIP { get; set; }

        private byte[] Buffer = new byte[SocketConfig.Instance.PacketSize];

        public void TriggerOnUDPMessage(byte[] data)
        {
            OnUDPMessage?.Invoke(this, data);
        }

        public ConnectedClientSocket(TcpClient tcpClient, UdpClient udpServer)
        {
            // init TCP

            TcpClient = tcpClient;

            UDPServer = udpServer;

            // init IPs

            TCPClientIP = (IPEndPoint)TcpClient.Client.RemoteEndPoint;

            UDPClientIP = new IPEndPoint(TCPClientIP.Address, TCPClientIP.Port + 10);

            TcpClient.GetStream().BeginRead(Buffer, 0, Buffer.Length, OnReadDone, null);
        }

        private void OnReadDone(IAsyncResult ar)
        {
            try
            {
                int Size = TcpClient.GetStream().EndRead(ar);

                if (Size <= 0)
                {
                    // connecion is closed
                    Close();
                    return;
                }

                byte[] trimmedData = new byte[Size];

                Array.Copy(Buffer, trimmedData, Size);

                OnTCPMessage?.Invoke(this, trimmedData);

                TcpClient.GetStream().BeginRead(Buffer, 0, Buffer.Length, OnReadDone, null);
            }
            catch (Exception ex)
            {
                Debug.Break();
                Debug.LogError(ex.Message);
            }
        }

        public void Close()
        {
            TcpClient.Dispose();
            UDPServer.Dispose();
            OnDisconnect?.Invoke(this);
        }

        /// <summary>
        /// Send raw data via TCP
        /// </summary>
        /// <param name="data"></param>
        public void SendTCP(byte[] data)
        {
            TcpClient.GetStream().Write(data, 0, data.Length);
        }


        /// <summary>
        /// Send raw data via UDP
        /// </summary>
        /// <param name="data"></param>
        public void SendUDP(byte[] data)
        {
            UDPServer.Send(data, data.Length, UDPClientIP);
        }
    }
}
