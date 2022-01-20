using Bloodthirst.Core.UILayout;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class LayoutRootBehaviour : LayoutBehaviour
{
    [SerializeField]
    private Color screenOutline;

    [SerializeField]
    private Color layoutOutline;

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private bool updateEachFrame;

    [SerializeField]
    private int flowCount;

    private int cachedPixelWidth;

    private int cachedPixelHeight;

    [Button]
    private void ResetChildRects()
    {
        LayoutBoxUtils.ResetChildrenRects(this);
    }

    [Button]
    private void ReloadChildren()
    {
        LayoutBoxUtils.AddChildren(this);
    }

    [Button]
    private void FlowRoot()
    {
        FlowLayoutEntry.FlowRoot(this);
    }
    private void OnValidate()
    {
        UpdateViewport();
    }

    private void UpdateViewport()
    {
        LayoutStyle.Width = new SizeUnit(cam.pixelWidth, UnitType.PIXEL);
        LayoutStyle.Height = new SizeUnit(cam.pixelHeight, UnitType.PIXEL);
        Rect.x = 0;
        Rect.y = 0;
        Rect.width = cam.pixelWidth;
        Rect.height = cam.pixelHeight;
    }

    private void Update()
    {
        if (cam.pixelHeight != cachedPixelHeight || cam.pixelWidth != cachedPixelWidth)
        {
            UpdateViewport();

            cachedPixelWidth = cam.pixelWidth;
            cachedPixelHeight = cam.pixelHeight;
        }

        if (updateEachFrame)
        {
            LayoutBoxUtils.ResetChildrenRects(this);
            LayoutBoxUtils.ClearChildren(this);
            LayoutBoxUtils.AddChildren(this);
            for (int i = 0; i < flowCount; i++)
            {
                FlowLayoutEntry.FlowRoot(this);
            }
        }
    }

    private void OnDrawGizmos()
    {
        DrawScreenOutline();

        DrawLayoutOutline();
    }

    private void DrawLayoutOutline()
    {
        Gizmos.color = layoutOutline;
        LayoutBoxUtils.DrawLayoutOutlines(this);
    }

    private void DrawScreenOutline()
    {
        float nearClip = cam.nearClipPlane;

        // bottom left
        Vector3 worldPos00 = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, nearClip));
        Handles.Label(worldPos00, "( 0 , 0 ) screen space");
        Gizmos.DrawSphere(Camera.main.ScreenToWorldPoint(worldPos00), 0.5f);

        // top right
        Vector3 worldPos11 = Camera.main.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, nearClip));
        Handles.Label(worldPos11, $"( {cam.pixelWidth} , { cam.pixelHeight} ) screen space");
        Gizmos.DrawSphere(Camera.main.ScreenToWorldPoint(worldPos11), 0.5f);

        // full screen outline
        Gizmos.color = screenOutline;
        LayoutBoxUtils.DrawRect(new Bloodthirst.Core.UILayout.Rect() { x = 0, y = 0, width = cam.pixelWidth, height = cam.pixelHeight });
    }
}
