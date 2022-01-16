using Sirenix.OdinInspector;
using UnityEngine;

namespace Bloodthirst.Core.UILayout
{
    public struct Rect
    {
        public Rect(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        [ShowInInspector]
        public float x { get; set; }

        [ShowInInspector]
        public float y { get; set; }

        [ShowInInspector]
        public float width { get; set; }

        [ShowInInspector]
        public float height { get; set; }
        public Vector2 center => new Vector2(x + (width * 0.5f), y + (height * 0.5f));
        public Vector2 size => new Vector2(width, height);
        public Vector2 topLeft => new Vector2(x, y);
        public Vector2 topRight => new Vector2(x + width, y);
        public Vector2 bottomLeft => new Vector2(x, y + height);
        public Vector2 bottomRight => new Vector2(x + width, y + height);
    }
}
