using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Socket.PacketParser
{
    public interface IPacketServerProcessor<TIdentifer> where TIdentifer : IComparable<TIdentifer>
    {
        /// <summary>
        /// Get Hash of the service used to recieve data of type T
        /// </summary>
        uint TypeIdentifier { get; }

        /// <summary>
        /// method called to parse the packet to type T , process it then broadcast it
        /// </summary>
        /// <param name="packet"></param>
        void Process(ConnectedClientSocket from , TIdentifer identifer , byte[] packet);
    }
}
