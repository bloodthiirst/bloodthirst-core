using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Bloodthirst.Editor.BNodeTree;
using Bloodthirst.Scripts.Utils;
using Bloodthirst;

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
        fromLocal = this.WorldToLocal(from.worldBound.center);
        toLocal = this.WorldToLocal(to.worldBound.center);

        Vector2 min = Vector2.Min(fromLocal, toLocal);
        Vector2 max = Vector2.Max(fromLocal, toLocal);

        style.left = min.x;
        style.top = min.y;

        style.width = max.x - min.x;
        style.height = max.y - min.y;

        MarkDirtyRepaint();
    }

    private void HandleContentGeneration(MeshGenerationContext cxt)
    {
        Vector2 start = Vector2.zero;
        Vector2 end = Vector2.zero;

        start = cxt.visualElement.WorldToLocal(From.worldBound.center);
        end = cxt.visualElement.WorldToLocal(To.worldBound.center);

        List<Vector2> points = new List<Vector2>
        {
            start,
            end
        };

        float lineLenght = 0;

        List<UIVertex> curve = VectorUtils.LineToCurve
            (
            points,
           UVSmoothingType.LERP,
            ref lineLenght,
             uvSmoothLerp: 0.5f,
             cornerSmoothing: 1,
             LineThikness: Thickness,
             handlesLength: (end - start).magnitude * 0.3f,
             detailPerSegment: 10,
             normalizeHandles: false,
             invertHandles: false
            );

        MeshWriteData mwd = cxt.Allocate(curve.Count, curve.Count, arrowTexture);

        // the vertices are setup with the center (0,0) being the top left corner of the rect of the VisualElement
        Vertex[] curveToVerts = curve.Select(v => new Vertex() { position = v.position, uv = v.uv0, tint = Color.white }).ToArray();

        float uvSliceX = lineLenght / Thickness;

        for (int i = 0; i < curveToVerts.Length; i++)
        {
            // fade color
            curveToVerts[i].tint = Color32.Lerp(FromColor, ToColor, curveToVerts[i].uv.x);

            // stretch uvs
            curveToVerts[i].uv.x *= uvSliceX;

            // fix depth
            curveToVerts[i].position.z = Vertex.nearZ;
        }


        mwd.SetAllVertices(curveToVerts);
        mwd.SetAllIndices(curveToVerts.Select((v, i) => (ushort)i).ToArray());
    }

}