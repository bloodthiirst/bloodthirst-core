using UnityEngine;
using UnityEngine.UI;

namespace Bloodthirst.Utils.EditorOpenTracker
{
    internal class DisableRaycastTargetAndMaskOnComponentAdded : IOnComponentAdded
    {
        void IOnComponentAdded.HandleComponentAdded(Component obj)
        {
            if (obj is Graphic graphic)
            {
                graphic.raycastTarget = false;
            }
            if (obj is MaskableGraphic maskable)
            {
                maskable.maskable = false;
            }
        }
    }
}
