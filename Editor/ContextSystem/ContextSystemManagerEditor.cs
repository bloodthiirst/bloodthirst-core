using Bloodthirst.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Bloodthirst.System.ContextSystem
{
    [InitializeOnLoad]
    public class ContextSystemManagerEditor
    {
        static ContextSystemManagerEditor()
        {
            if (!EditorConsts.ON_ASSEMBLY_RELOAD_CONTEXT_SYSTEM)
                return;

            EditorApplication.playModeStateChanged -= HandlePlayModeChanged;
            EditorApplication.playModeStateChanged += HandlePlayModeChanged;
        }

        internal static void Initialize()
        {
            IContextInstance[] allContexts = Resources.LoadAll("Singletons", typeof(IContextInstance)).OfType<IContextInstance>().Where(so => so != null).ToArray();
            ContextSystemManager.SetAllContexts(allContexts);
        }

        private static void HandlePlayModeChanged(PlayModeStateChange obj)
        {
            Initialize();
        }
    }
}
