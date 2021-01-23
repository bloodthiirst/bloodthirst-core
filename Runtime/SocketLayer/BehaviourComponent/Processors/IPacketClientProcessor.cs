using System;

namespace Bloodthirst.Socket.BehaviourComponent
{
    public interface IPacketClientProcessor<TIdentifier> where TIdentifier : IComparable<TIdentifier>
    {
        bool ProcessPacket(uint type, TIdentifier from, byte[] data);
    }
}
