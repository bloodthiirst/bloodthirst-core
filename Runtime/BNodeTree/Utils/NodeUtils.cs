using System.Collections.Generic;

namespace Bloodthirst.Runtime.BNodeTree
{
    public static class NodeUtils
    {

        public static IEnumerable<IPortType> GetPorts(this INodeType nodeType, PORT_DIRECTION direction, PORT_TYPE type)
        {
            foreach (IPortType p in nodeType.Ports)
            {
                if (p.PortDirection == direction && p.PortType == type)
                {
                    yield return p;
                }
            }
        }

        public static IEnumerable<IPortType> GetPorts(this INodeType nodeType, PORT_DIRECTION direction)
        {
            foreach (IPortType p in nodeType.Ports)
            {
                if (p.PortDirection == direction)
                    yield return p;
            }
        }

    }
}

