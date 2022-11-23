using Bloodthirst.Core.UI;
using Bloodthirst.Editor;
using System;
using System.Collections;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class UILineRendererInspector : VisualElement
{
    private const string UXML_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/LineRenderer.Editor/Inspector/UILineRendererInspector.uxml";
    private const string USS_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/LineRenderer.Editor/Inspector/UILineRendererInspector.uss";

    private VisualElement UISettings => this.Q<VisualElement>(nameof(UISettings));
    private VisualElement SplineSettings => this.Q<VisualElement>(nameof(SplineSettings));
    private VisualElement Custom => this.Q<VisualElement>(nameof(Custom));
    private VisualElement ListContainer => this.Q<VisualElement>(nameof(ListContainer));
    private Label CountTxt => this.Q<Label>(nameof(CountTxt));
    private Button AddBtn => this.Q<Button>(nameof(AddBtn));
    private ObjectField MaterialField { get; set; }
    private ColorField ColorField { get; set; }
    private Vector4Field RaycastTargetPadding { get; set; }
    private Toggle RaycastTarget { get; set; }
    private Toggle Maskable { get; set; }

    private IntegerField DetailsPerSegment { get; set; }
    private FloatField LineThicness { get; set; }
    private EnumField UVSmoothing { get; set; }
    private Slider UVSmoothLerp { get; set; }
    private SliderInt CornerSmoothing { get; set; }

    private ListView PointsList { get; set; }
    public UILineRendererEditor Editor { get; }
    public UILineRenderer UILineRenderer { get; }

    public UILineRendererInspector( UILineRendererEditor editor , UILineRenderer uiLineRenderer)
    {
        Editor = editor;
        UILineRenderer = uiLineRenderer;

        VisualTreeAsset uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
        StyleSheet style = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);

        uxml.CloneTree(this);

        if (!styleSheets.Contains(EditorConsts.GlobalStyleSheet))
        {
            styleSheets.Add(EditorConsts.GlobalStyleSheet);
        }
        styleSheets.Add(style);

        // ui settings
        MaterialField = new ObjectField("Material");
        MaterialField.objectType = typeof(Material);
        MaterialField.bindingPath = "m_Material";
        UISettings.Add(MaterialField);

        ColorField = new ColorField("Color");
        ColorField.bindingPath = "m_Color";
        UISettings.Add(ColorField);

        RaycastTargetPadding = new Vector4Field("Raycast Padding");
        RaycastTargetPadding.bindingPath = "m_RaycastPadding";
        UISettings.Add(RaycastTargetPadding);

        RaycastTarget = new Toggle("Raycast Target");
        RaycastTarget.bindingPath = "m_RaycastTarget";
        UISettings.Add(RaycastTarget);

        Maskable = new Toggle("Maskable");
        Maskable.bindingPath = "m_Maskable";
        UISettings.Add(Maskable);

        // spline settings
        DetailsPerSegment = new IntegerField("Resolution");
        DetailsPerSegment.bindingPath = "resolution";
        SplineSettings.Add(DetailsPerSegment);

        CornerSmoothing = new SliderInt("Corner Smoothing");
        CornerSmoothing.bindingPath = "cornerSmoothing";
        SplineSettings.Add(CornerSmoothing);

        LineThicness = new FloatField("LineThicness");
        LineThicness.bindingPath = "lineThickness";
        SplineSettings.Add(LineThicness);

        UVSmoothing = new EnumField("UVSmoothing");
        UVSmoothing.bindingPath = "uvSmoothing";
        SplineSettings.Add(UVSmoothing);

        UVSmoothLerp = new Slider("UV Smooth Lerp");
        UVSmoothLerp.bindingPath = "uvSmoothLerp";
        SplineSettings.Add(UVSmoothLerp);

        // bind
        SerializedObject so = new SerializedObject(UILineRenderer);



        UISettings.Bind(so);
        SplineSettings.Bind(so);

        InitializeUI();

    }

    public void Unbind()
    {
        UISettings.Unbind();
        SplineSettings.Unbind();
    }

    private void InitializeUI()
    {
        PointsList = new ListView((IList)UILineRenderer.Points, 30, MakeListItem, BindListItem);
        ListContainer.Add(PointsList);

        AddBtn.clickable.clicked -= HandleAddPointClicked;
        AddBtn.clickable.clicked += HandleAddPointClicked;

    }

    private void HandleAddPointClicked()
    {
        var last = UILineRenderer.Points[UILineRenderer.Points.Count - 1];
        var before = UILineRenderer.Points[UILineRenderer.Points.Count - 4];

        var final = last + (last - before);
        var h1 = Vector3.Lerp(last, final, 0.33f);
        var h2 = Vector3.Lerp(last, final, 0.66f);


        UILineRenderer.AddPoint(h1);
        UILineRenderer.AddPoint(h2);
        UILineRenderer.AddPoint(final);

        PointsList.RefreshItems();
    }

    internal void RefreshPoint(int index)
    {
        PointsList.RefreshItem(index);
    }

    internal void RefreshPoints()
    {
        PointsList.RefreshItems();
    }

    private void BindListItem(VisualElement elemUi, int index)
    {
        PointElement casted = (PointElement)elemUi;
        casted.Index = index;

        casted.Refresh();
    }

    private VisualElement MakeListItem()
    {
        return new PointElement(Editor);
    }
}
