using Bloodthirst.Core.UI;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(UILineRenderer))]
public class UILineRendererEditor : Editor
{
    internal UILineRenderer LineRenderer { get; set; }
    internal UILineRendererInspector Inspector { get; set; }
    internal UILineRendererSceneView SceneView { get; set; }

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        LineRenderer = target as UILineRenderer;
        SceneView = new UILineRendererSceneView(this, LineRenderer);
        Inspector = new UILineRendererInspector(this, LineRenderer);

        SceneView.OnEnable();
    }
    private void OnDisable()
    {
        SceneView.OnDisable();
    }
    private void OnDestroy()
    {
        if (Inspector != null)
        {
            // unbind
            Inspector.Unbind();
        }
    }

    public override VisualElement CreateInspectorGUI()
    {
        return Inspector;
    }

    private void OnSceneGUI()
    {
        SceneView.OnSceneGUI();
    }

    internal void RemovePoint(int index)
    {
        if(LineRenderer.Points.Count < 5)
        {
            EditorUtility.DisplayDialog("Error", "You can't have less than 2 points in a line" , "Ok");
            return;
        }

        int type = index % 3;

        if(type != 0)
        {
            EditorUtility.DisplayDialog("Error", "You can't remove Handle vertices from the line , only point verticies are removable ", "Ok");
            return;
        }

        LineRenderer.RemovePoint(index);
        Inspector.RefreshPoints();
    }
}
