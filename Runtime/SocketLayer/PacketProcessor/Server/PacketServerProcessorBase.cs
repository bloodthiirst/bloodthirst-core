using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Socket.PacketParser
{
    public abstract class PacketServerProcessorBase<TIdentifier> : IPacketServerProcessor<TIdentifier> where TIdentifier : IComparable<TIdentifier>
    {
        /// <summary>
        /// Get Hash of the service used to recieve data of type T
        /// </summary>
        protected readonly uint typeIdentifier;

        public uint TypeIdentifier => typeIdentifier;

        /// <summary>
        /// method called to parse the packet to type T , process it then broadcast it
        /// </summary>
        /// <param name="packet"></param>
        public abstract void Process( ConnectedClientSocket from , TIdentifier identifier, byte[] packet);

        public PacketServerProcessorBase(uint typeIdentifier)
        {
            this.typeIdentifier = typeIdentifier;
        }
    }
}
