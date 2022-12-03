#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
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

        #if ODIN_INSPECTOR[ShowInInspector]#endif
        public float x { get; set; }

        #if ODIN_INSPECTOR[ShowInInspector]#endif
        public float y { get; set; }

        #if ODIN_INSPECTOR[ShowInInspector]#endif
        public float width { get; set; }

        #if ODIN_INSPECTOR[ShowInInspector]#endif
        public float height { get; set; }
        public Vector2 center => new Vector2(x + (width * 0.5f), y + (height * 0.5f));
        public Vector2 size => new Vector2(width, height);
        public Vector2 topLeft => new Vector2(x, y);
        public Vector2 topRight => new Vector2(x + width, y);
        public Vector2 bottomLeft => new Vector2(x, y + height);
        public Vector2 bottomRight => new Vector2(x + width, y + height);
    }
}
