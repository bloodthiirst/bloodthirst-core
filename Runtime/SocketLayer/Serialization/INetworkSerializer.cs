using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodthirst.Socket.Serializer
{
    public interface INetworkSerializer<TData>
    {
        int StartIndex { get; }

        int Length { get; }

        byte[] Serialize(TData identifier);

        TData Deserialize(byte[] data);
    }
}
