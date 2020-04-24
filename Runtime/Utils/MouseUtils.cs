using Bloodthirst.Core.UnitySingleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Core.Utils
{
    public class MouseUtils : UnitySingleton<MouseUtils>
    {
        public Vector3 MouseDelta { get; private set; }

        public bool MouseMoved => MouseDelta != Vector3.zero;

        private Vector3 mouseLastFrame;

        private Vector2 normalizedMousePosition;

        /// <summary>
        /// Get the normalized mouse screen position ( -1 , 1 )
        /// x : normalized width
        /// y : normalized height
        /// </summary>
        public Vector2 NormalizedMousePosition => normalizedMousePosition;

        private void Update()
        {
            MouseDelta = Input.mousePosition - mouseLastFrame;

            mouseLastFrame = Input.mousePosition;

            normalizedMousePosition.x = ((Input.mousePosition.x / Screen.width) * 2 ) - 1;

            normalizedMousePosition.y = ((Input.mousePosition.y / Screen.height) * 2) - 1;
        }
    }
}
