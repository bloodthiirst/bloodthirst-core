using Sirenix.OdinInspector;
using UnityEngine;

namespace Bloodthirst.Editor.BHotReload
{
    public class BHotReloadSettings : ScriptableObject
    {
        [SerializeField]
        public bool enabled;
        
        [SerializeField]
        public bool debugMode;

        [Button(ButtonSizes.Large , ButtonStyle.Box)]
        private void Recompile()
        {
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation(UnityEditor.Compilation.RequestScriptCompilationOptions.None);
        }
    }
}
