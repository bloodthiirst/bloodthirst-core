using System;

namespace Bloodthirst.Socket.PacketParser
{
    public abstract class PacketClientProcessorBase<TIdentifier> : IPacketClientProcessor<TIdentifier> where TIdentifier : IComparable<TIdentifier>
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
        public abstract void Process(TIdentifier from, byte[] packet);

        public PacketClientProcessorBase(uint typeIdentifier)
        {
            this.typeIdentifier = typeIdentifier;
        }
    }
}
