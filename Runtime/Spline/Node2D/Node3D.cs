
using UnityEngine;

namespace Bloodthirst.Utils
{
    public class Node3D : INodePosition
    {
        public Vector3 point;
        public Vector3 Position => point;

        public static implicit operator Node3D(Vector3 vector2)
        {
            return new Node3D() { point = vector2 };
        }

        public static implicit operator Vector3(Node3D node)
        {
            return node.point;
        }
    }
}
