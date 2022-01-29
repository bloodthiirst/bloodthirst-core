using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Bloodthirst.Editor.BInspector;

namespace Bloodthirst.Editor.BSearch
{
    public class BSearch : EditorWindow
    {
        private const string UXML_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/BSearch/BSearch.uxml";
        private const string USS_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/BSearch/BSearch.uss";

        [MenuItem("Bloodthirst Tool/BSearch")]
        public static void ShowExample()
        {
            BSearch wnd = GetWindow<BSearch>();
            wnd.titleContent = new GUIContent("BSearch");
        }

        private EditorTest t;

        private void Awake()
        {
            t = new EditorTest();
        }
        private void OnEnable()
        {
            t = new EditorTest();
        }

        public void CreateGUI()
        {
            // root
            VisualElement root = rootVisualElement;

            root.Clear();

            // uxml
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
            VisualElement uxml = visualTree.Instantiate();

            // stylesheet
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);

            root.styleSheets.Add(styleSheet);
            root.Add(uxml);

            IBInspectorDrawer editorCreator = BInspectorProvider.DefaultInspector;

            VisualElement insp =  editorCreator.CreateInspectorGUI(t);

            root.Add(insp);
        }
    }
}