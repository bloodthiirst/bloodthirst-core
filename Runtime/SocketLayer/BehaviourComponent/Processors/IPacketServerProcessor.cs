using System;

namespace Bloodthirst.Socket.BehaviourComponent
{
    public interface IPacketServerProcessor<TIdentifier> where TIdentifier : IComparable<TIdentifier>
    {
        bool ProcessPacket(uint type, ConnectedClientSocket connectedClient, TIdentifier from, byte[] data);
    }
}
