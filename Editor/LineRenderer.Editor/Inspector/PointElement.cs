using Bloodthirst.Core.UI;
using Bloodthirst.Core.Utils;
using Bloodthirst.Editor;
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class PointElement : VisualElement
{
    private const string POINT_CLASS = "point-bg";
    private const string HANDLE_CLASS = "handle-bg";
    private const string UXML_ELEMENT_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/LineRenderer.Editor/Inspector/PointElement.uxml";
    public Label Title { get; set; }
    public Vector2Field PointValue { get; set; }
    public Button RemoveBtn { get; set; }
    public UILineRendererEditor Editor { get; set; }
    public int Index { get; set; }

    public PointElement(UILineRendererEditor editor)
    {
        Editor = editor;

        VisualTreeAsset uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_ELEMENT_PATH);
        uxml.CloneTree(this);

        Title = this.Q<Label>(nameof(Title));
        PointValue = this.Q<Vector2Field>(nameof(PointValue));
        RemoveBtn = this.Q<Button>(nameof(RemoveBtn));

        RemoveBtn.clickable.clicked -= HandleRemoveClicked;
        RemoveBtn.clickable.clicked += HandleRemoveClicked;

        PointValue.UnregisterValueChangedCallback(HandleValueChanged);
        PointValue.RegisterValueChangedCallback(HandleValueChanged);
    }

    private void HandleRemoveClicked()
    {
        Editor.RemovePoint(Index);
    }

    private void HandleValueChanged(ChangeEvent<Vector2> evt)
    {
        Editor.LineRenderer.UpdatePoint(evt.newValue, Index);
    }

    internal void Refresh()
    {
        Vector2 newValue = Editor.LineRenderer.Points[Index];
        PointValue.SetValueWithoutNotify(newValue);

        int type = Index % 3;

        RemoveFromClassList(POINT_CLASS);
        RemoveFromClassList(HANDLE_CLASS);

        switch (type)
        {
            case 0:
                {
                    Title.text = $"Point { Index / 3 }";
                    AddToClassList(POINT_CLASS);
                    RemoveBtn.Display(true);
                    break;
                }
            case 1:
                {
                    Title.text = "Handle 1";

                    AddToClassList(HANDLE_CLASS);
                    RemoveBtn.Display(false);
                    break;
                }
            case 2:
                {
                    Title.text = "Handle 2";
                    AddToClassList(HANDLE_CLASS);
                    RemoveBtn.Display(false);
                    break;
                }
            default:
                break;
        }
    }
}
