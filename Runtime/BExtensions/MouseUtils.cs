using UnityEngine;

namespace Bloodthirst.Scripts.Core.Utils
{
    public class MouseUtils : MonoBehaviour
    {
        /// <summary>
        /// Mouse Delta from last frame to current frame
        /// </summary>
        public Vector3 MouseDelta { get; private set; }

        /// <summary>
        /// Did the mouse move from the last frame to the current frame ?
        /// </summary>
        public bool MouseMoved => MouseDelta != Vector3.zero;

        private Vector3 mouseLastFrame;

        private Vector2 normalizedMousePosition;

        /// <summary>
        /// Get the normalized mouse screen position ( -1 , 1 )
        /// x : normalized width
        /// y : normalized height
        /// </summary>
        public Vector2 NormalizedMousePosition => normalizedMousePosition;

        /// <summary>
        /// Mouse scoll delta
        /// if value is postive : scroll up , if value is negative then it's scroll down
        /// </summary>
        public float MouseScrollDelta { get; private set; }

        /// <summary>
        /// Mouse position in screen pixel coords
        /// x : ( 0 -> width)
        /// y : ( 0 -> height)
        /// </summary>
        public Vector2 MousePosition { get; private set; }

        private void Update()
        {
            MousePosition = Input.mousePosition;

            MouseScrollDelta = -Input.mouseScrollDelta.y;

            MouseDelta = Input.mousePosition - mouseLastFrame;

            mouseLastFrame = Input.mousePosition;

            normalizedMousePosition.x = ((Input.mousePosition.x / Screen.width) * 2) - 1;

            normalizedMousePosition.y = ((Input.mousePosition.y / Screen.height) * 2) - 1;
        }
    }
}
