using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Bloodthirst.Editor.BNodeTree;
using Bloodthirst.Scripts.Utils;
using Bloodthirst;
using UnityEngine.Pool;
using Unity.Collections;
using Unity.EditorCoroutines.Editor;
using System.Collections;
using System;
using Bloodthirst.Core.Utils;

public class UILinelement : VisualElement
{
    VisualElement from;
    Vector2 fromLocal;

    VisualElement to;
    Vector2 toLocal;

    private const string ARROW_TEXTURE = BNodeTreeEditorUtils.EDITOR_BASE_PATH + "/Resources/ArrowDirection.png";

    private Texture2D arrowTexture;

    /// <summary>
    /// From position in world space
    /// </summary>
    public VisualElement From
    {
        get { return from; }
        set
        {
            if (from == value)
                return;

            from = value;
        }
    }


    /// <summary>
    /// To position in world space
    /// </summary>
    public VisualElement To
    {
        get { return to; }
        set
        {
            if (to == value)
                return;

            to = value;
        }
    }

    public float Thickness { get; }
    public Color FromColor { get; set; }
    public Color ToColor { get; set; }

    public UILinelement(float thickness = 20f)
    {
        name = "Link";
        usageHints = UsageHints.DynamicColor | UsageHints.DynamicTransform;
        arrowTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(ARROW_TEXTURE);
        generateVisualContent += HandleContentGeneration;

        Thickness = thickness;
    }

    public void Refresh()
    {
        UpdateBounds();
    }

    void UpdateBounds()
    {
        fromLocal = this.parent.WorldToLocal(from.worldBound.center);
        toLocal = this.parent.WorldToLocal(to.worldBound.center);

        Vector2 min = Vector2.Min(fromLocal, toLocal);
        Vector2 max = Vector2.Max(fromLocal, toLocal);

        style.left = (int)min.x;
        style.top = (int)min.y;

        style.width = (int)(max.x - min.x);
        style.height = (int)(max.y - min.y);

        MarkDirtyRepaint();
        //EditorCoroutineUtility.StartCoroutineOwnerless(CrtWaitLayout());
    }

    private IEnumerator CrtWaitLayout()
    {
        while (!this.IsLayoutBuilt())
        {
            yield return null;
        }

        MarkDirtyRepaint();
    }

    private void HandleContentGeneration(MeshGenerationContext ctx)
    {
        Vector2 start = Vector2.zero;
        Vector2 end = Vector2.zero;

        start = ctx.visualElement.WorldToLocal(From.worldBound.center);
        end = ctx.visualElement.WorldToLocal(To.worldBound.center);


        float lineLength = 0;

        LineUtils.CurveSettings settings = new LineUtils.CurveSettings()
        {
            UVSmoothing = UVSmoothingType.LERP,
            UVSmoothLerp = 0.5f,
            CornerSmoothing = 1,
            LineThikness = Thickness,
            HandlesLength = (end - start).magnitude * 0.3f,
            Resolution = 10,
            NormalizeHandles = false,
            InvertHandles = false
        };

        using (ListPool<Vector2>.Get(out List<Vector2> points))
        using (ListPool<UIVertex>.Get(out List<UIVertex> curve))
        using (ListPool<int>.Get(out List<int> indicies))
        {
            points.Add(start);
            points.Add(end);

            LineUtils.PointsToCurve(points, settings, out lineLength, ref curve, ref indicies);

            for (int i = 0; i < curve.Count / 2; i++)
            {
                int otherIndex = curve.Count - 1 - i;
                UIVertex curr = curve[i];
                curve[i] = curve[otherIndex];
                curve[otherIndex] = curr;
            }

            MeshWriteData mwd = ctx.Allocate(curve.Count, indicies.Count, arrowTexture);

            // the vertices are setup with the center (0,0) being the top left corner of the rect of the VisualElement
            NativeArray<Vertex> curveToVerts = new NativeArray<Vertex>(curve.Count, Allocator.Temp);

            for (int i = 0; i < curve.Count; ++i)
            {
                UIVertex curr = curve[i];
                curveToVerts[i] = new Vertex() { position = curr.position, uv = curr.uv0, tint = Color.white };
            }

            float uvSliceX = lineLength / Thickness;

            for (int i = 0; i < curveToVerts.Length; i++)
            {
                Vertex curr = curveToVerts[i];

                // fade color
                curr.tint = Color32.Lerp(FromColor, ToColor, curveToVerts[i].uv.x);

                // stretch uvs
                curr.uv.x *= uvSliceX;

                // fix depth
                curr.position.z = Vertex.nearZ;

                curveToVerts[i] = curr;
            }

            mwd.SetAllVertices(curveToVerts);

            NativeArray<ushort> tmp = new NativeArray<ushort>(indicies.Count, Allocator.Temp);

            for (int i = 0; i < indicies.Count; i++)
            {
                int ind = indicies[i];
                tmp[i] = (ushort)ind;
            }
            
            mwd.SetAllIndices(tmp);
        }
    }

}