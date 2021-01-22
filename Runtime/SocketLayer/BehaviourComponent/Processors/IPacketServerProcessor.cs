using Bloodthirst.Socket;
using System;

namespace Assets.SocketLayer.BehaviourComponent
{
    public interface IPacketServerProcessor<TIdentifier> where TIdentifier : IComparable<TIdentifier>
    {
        bool ProcessPacket(uint type, ConnectedClientSocket connectedClient, TIdentifier from, byte[] data);
    }
}
