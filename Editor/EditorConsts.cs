using UnityEditor;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor
{
    public class EditorConsts
    {
        public const string GLOBAL_EDITOR_FOLRDER_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/";

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
