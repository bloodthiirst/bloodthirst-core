#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace Bloodthirst.Editor.BHotReload
{
    public class BHotReloadSettings : ScriptableObject
    {
        [SerializeField]
        public bool enabled;
        
        [SerializeField]
        public bool debugMode;

#if ODIN_INSPECTOR
        [Button(ButtonSizes.Large , ButtonStyle.Box)]
#endif
        private void Recompile()
        {
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation(UnityEditor.Compilation.RequestScriptCompilationOptions.None);
        }
    }
}
