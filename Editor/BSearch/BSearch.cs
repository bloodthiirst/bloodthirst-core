using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class BSearch : EditorWindow
{
    [MenuItem("Bloodthirst Tool/BSearch")]
    public static void ShowExample()
    {
        BSearch wnd = GetWindow<BSearch>();
        wnd.titleContent = new GUIContent("BSearch");

        //wnd.ShowPopup();
        //wnd.ShowAsDropDown(new Rect( 300 , 0 , 200 , 100), Vector2.one * 300);
        //wnd.ShowUtility();
        //wnd.ShowModalUtility();
        //wnd.ShowAuxWindow();

        wnd.ShowNotification(new GUIContent("Hey Listen !"), 1f);
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.bloodthirst.bloodthirst-core/Editor/BSearch/BSearch.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.bloodthirst.bloodthirst-core/Editor/BSearch/BSearch.uss");
        VisualElement labelWithStyle = new Label("Hello World! With Style");
        labelWithStyle.styleSheets.Add(styleSheet);
        root.Add(labelWithStyle);
    }
}