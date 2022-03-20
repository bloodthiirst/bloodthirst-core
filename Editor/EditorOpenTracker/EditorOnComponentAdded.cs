using UnityEditor;
using UnityEngine.UI;

namespace Bloodthirst.Utils.EditorOpenTracker
{
    [InitializeOnLoad]
    public class EditorOnComponentAdded
    {
        static EditorOnComponentAdded()
        {
            ObjectFactory.componentWasAdded -= HandleComponentAdded;
            ObjectFactory.componentWasAdded += HandleComponentAdded;

            EditorApplication.quitting -= OnEditorQuiting;
            EditorApplication.quitting += OnEditorQuiting;
        }
        private static void HandleComponentAdded(UnityEngine.Component obj)
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

        private static void OnEditorQuiting()
        {
            ObjectFactory.componentWasAdded -= HandleComponentAdded;
            EditorApplication.quitting -= OnEditorQuiting;
        }
    }
}
