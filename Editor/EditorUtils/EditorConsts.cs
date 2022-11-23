using UnityEditor;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor
{
    public class EditorConsts
    {

        /// <summary>
        /// NOTE : AdvancedTypeCache depends of the ScriptWatcher , toggle it on if you want to use it
        /// </summary>
        public static readonly bool ON_ASSEMBLY_RELOAD_GAME_EVENT_SCRIPT_WATCHER = false;

        /// <summary>
        /// NOTE : AdvancedTypeCache depends of the ScriptWatcher , toggle it on if you want to use it
        /// </summary>
        public static readonly bool ON_ASSEMBLY_RELOAD_ADVANCED_TYPE_CACHE = true;
        
        public static readonly bool ON_ASSEMBLY_RELOAD_SCRIPTWATCHER = false;
        public static readonly bool ON_ASSEMBLY_RELOAD_BADAPTER = false;
        public static readonly bool ON_ASSEMBLY_RELOAD_BHOTRELOAD = false;
        public static readonly bool ON_ASSEMBLY_RELOAD_BINSPECTOR_EDITOR = true;
        public static readonly bool ON_ASSEMBLY_RELOAD_COMMAND_MANAGER_EDITOR = false;
        public static readonly bool ON_ASSEMBLY_RELOAD_ON_COMPONENT_ADDED = false;
        public static readonly bool ON_ASSEMBLY_RELOAD_STARTUP_TRACKER = false;
        public static readonly bool ON_ASSEMBLY_RELOAD_SCENE_DEPENDENCY_INJECTOR = false;
        public static readonly bool ON_ASSEMBLY_RELOAD_BISD_SAVE_MANAGER = false;
        public static readonly bool ON_ASSEMBLY_RELOAD_CONTEXT_SYSTEM = false;

        public const string GLOBAL_EDITOR_FOLRDER_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/";
        public const string IS_PLAYING = "@UnityEngine.Application.isPlaying";

        private const string GLBOAL_USS_PATH = GLOBAL_EDITOR_FOLRDER_PATH + "GlobalStyleSheet.uss";

        private static StyleSheet globalStyleSheet;

        public static StyleSheet GlobalStyleSheet
        {
            get
            {
                if (globalStyleSheet == null)
                {
                    globalStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(GLBOAL_USS_PATH);
                }

                return globalStyleSheet;
            }
        }
    }
}
