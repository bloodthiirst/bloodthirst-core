using UnityEngine;

namespace Bloodthirst.Core.UILayout
{
    public abstract class UICanvasInfoBase
    {
        public abstract Vector3 CanvasToWorld(Vector2 canvasPos);
        public abstract Vector2 WorldToCanvas(Vector3 worldPos);

    }

    public class UICanvasCamera : UICanvasInfoBase
    {
        private Camera cam;

        public Camera Cam => cam;
        public UICanvasCamera(Camera cam)
        {
            this.cam = cam;
        }


        public override Vector3 CanvasToWorld(Vector2 canvasPos)
        {
            Vector3 vec = canvasPos;
            vec.z = Cam.nearClipPlane;
            Vector3 worldPos = Cam.ScreenToWorldPoint(vec);
            return worldPos;
        }
        public override Vector2 WorldToCanvas(Vector3 worldPos)
        {
            return Cam.WorldToScreenPoint(worldPos);
        }

    }
}
