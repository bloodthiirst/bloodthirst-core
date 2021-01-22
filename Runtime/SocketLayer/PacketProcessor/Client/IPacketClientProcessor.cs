using System;

namespace Bloodthirst.Socket.PacketParser
{
    public interface IPacketClientProcessor<TIdentifer> where TIdentifer : IComparable<TIdentifer>
    {
        /// <summary>
        /// Get Hash of the service used to recieve data of type T
        /// </summary>
        uint TypeIdentifier { get; }

        /// <summary>
        /// method called to parse the packet to type T , process it then broadcast it
        /// </summary>
        /// <param name="packet"></param>
        void Process(TIdentifer from, byte[] packet);
    }
}
