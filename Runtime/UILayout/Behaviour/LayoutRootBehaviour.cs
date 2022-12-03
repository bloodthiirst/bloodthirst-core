using Bloodthirst.Core.UILayout;
#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using System.Collections.Generic;
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

    #if ODIN_INSPECTOR[Button]#endif
    private void ResetChildRects()
    {
        LayoutBoxUtils.ResetChildrenRects(this);
    }

    #if ODIN_INSPECTOR[Button]#endif
    private void ReloadChildren()
    {
        LayoutBoxUtils.AddChildren(this);
    }

    #if ODIN_INSPECTOR[Button]#endif
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

    private void OnEnable()
    {
        SceneView.duringSceneGui -= HandleSceneView;
        SceneView.duringSceneGui += HandleSceneView;
    }


    private void OnDisable()
    {
        SceneView.duringSceneGui -= HandleSceneView;
    }

    private void HandleSceneView(SceneView currentSv)
    {
        UICanvasInfoBase canvasInfo = new UICanvasCamera(cam);

        DrawScreenOutline(currentSv, canvasInfo);
        DrawLayoutOutline(currentSv, canvasInfo);
        DrawMousePosition(currentSv, canvasInfo);
        HighlightMouseHover(currentSv, canvasInfo);

        currentSv.Repaint();
    }

    private void HighlightMouseHover(SceneView sv, UICanvasInfoBase canvasInfo)
    {
        if (!TryGetMousePositionCanvasSpace(sv, canvasInfo, out Vector2 canvasPos, out Vector3 worldPos))
            return;

        List<ILayoutBox> layouts = new List<ILayoutBox>();

        foreach (ILayoutBox c in ChildLayouts)
        {
            RecursiveFindRectThatContainPoint(c, canvasPos, layouts);
        }

        Handles.color = Color.red;
        foreach (ILayoutBox l in layouts)
        {
            LayoutBoxUtils.DrawSingleLayout(l, sv, canvasInfo);
        }

    }

    private void RecursiveFindRectThatContainPoint(ILayoutBox layoutBox, Vector2 canvasPos, List<ILayoutBox> layouts)
    {
        if (LayoutBoxUtils.IsInsideRect(layoutBox.Rect, canvasPos))
        {
            layouts.Add(layoutBox);
        }

        foreach (ILayoutBox c in layoutBox.ChildLayouts)
        {
            RecursiveFindRectThatContainPoint(c, canvasPos, layouts);
        }
    }

    public bool TryGetMousePositionCanvasSpace(SceneView sv, UICanvasInfoBase canvasInfo, out Vector2 canvasPos, out Vector3 worldPos)
    {
        UICanvasCamera canvasCam = (UICanvasCamera)canvasInfo;
        Plane plane = new Plane(-canvasCam.Cam.transform.forward, canvasCam.Cam.transform.position + (canvasCam.Cam.transform.forward * canvasCam.Cam.nearClipPlane));

        Vector3 mousePosition = Event.current.mousePosition;
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);

        if (!plane.Raycast(ray, out float d))
        {
            canvasPos = Vector2.zero;
            worldPos = Vector3.zero;
            return false;
        }

        worldPos = ray.GetPoint(d);
        canvasPos = canvasInfo.WorldToCanvas(worldPos);

        return true;
    }

    private void DrawMousePosition(SceneView currentSv, UICanvasInfoBase canvasInfo)
    {
        if (!TryGetMousePositionCanvasSpace(currentSv, canvasInfo, out Vector2 canvasPos, out Vector3 worldPos))
            return;

        Handles.color = Color.white;
        Handles.DrawSolidDisc(worldPos, currentSv.camera.transform.forward, HandleUtility.GetHandleSize(worldPos) * 0.05f);
        Handles.Label(worldPos, $"( { canvasPos.x: 0} , {canvasPos.y: 0} ) Mouse");
    }

    private void DrawLayoutOutline(SceneView currentSv, UICanvasInfoBase canvasInfo)
    {
        Handles.color = layoutOutline;
        LayoutBoxUtils.DrawLayoutOutlines(this, currentSv, canvasInfo);
    }

    private void DrawScreenOutline(SceneView sv, UICanvasInfoBase canvasInfo)
    {
        // bottom left
        Vector3 worldPosBottomLeft = canvasInfo.CanvasToWorld(Vector2.zero);
        Handles.Label(worldPosBottomLeft, "( 0 , 0 ) screen space");
        Handles.DrawSolidDisc(worldPosBottomLeft, sv.camera.transform.forward, HandleUtility.GetHandleSize(worldPosBottomLeft) * 0.1f);

        // top right
        Vector3 worldPosTopRight = canvasInfo.CanvasToWorld(new Vector2(cam.pixelWidth, cam.pixelHeight));
        Handles.Label(worldPosTopRight, $"( {cam.pixelWidth} , { cam.pixelHeight} ) screen space");
        Handles.DrawSolidDisc(worldPosTopRight, sv.camera.transform.forward, HandleUtility.GetHandleSize(worldPosTopRight) * 0.1f);

        // full screen outline
        Handles.color = screenOutline;
        LayoutBoxUtils.DrawRect(new Bloodthirst.Core.UILayout.Rect() { x = 0, y = 0, width = cam.pixelWidth, height = cam.pixelHeight }, sv, canvasInfo);
    }
}
