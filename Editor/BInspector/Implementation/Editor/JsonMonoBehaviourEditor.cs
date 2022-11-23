using Bloodthirst.JsonUnityObject;
using UnityEditor;
using UnityEngine.UIElements;
using static Bloodthirst.Editor.BInspector.BInspectorDefault;

namespace Bloodthirst.Editor.BInspector
{
    /// <summary>
    /// <para>The base editor class used to create the inspectors for components that inherint from <see cref="JsonMonoBehaviour"/> </para>
    /// </summary>
    [CustomEditor(typeof(JsonMonoBehaviour), true)]
    public class JsonMonoBehaviourEditor : UnityEditor.Editor
    {
        private BInspectorDefault.RootEditor rootEditor;

        public override VisualElement CreateInspectorGUI()
        {
            IBInspectorDrawer editorCreator = BInspectorProvider.Get(target);

            if(editorCreator == null)
            {
                editorCreator = BInspectorProvider.DefaultInspector;
            }

            rootEditor = editorCreator.CreateInspectorGUI(target);

            return rootEditor.RootContainer;
        }

        private void OnDestroy()
        {
            if (rootEditor.RootDrawer == null)
                return;

            ValueDrawerUtils.DoDestroyRoot(rootEditor.RootDrawer);
        }
    }
}
