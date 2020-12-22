
using UnityEngine;

namespace Bloodthirst.Utils
{
    public class Node2D : INodePosition
    {
        public Vector2 point;
        public Vector3 Position => point;

        public static implicit operator Node2D(Vector2 vector2)
        {
            return new Node2D() { point = vector2 };
        }

        public static implicit operator Vector2(Node2D node)
        {
            return node.point;
        }
    }
}
