/**
using Bloodthirst.Utils;
using Packages.com.bloodthirst.bloodthirst_core.Runtime.UI.UILineRenderer;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Bloodthirst
{
    [CustomEditor(typeof(PathMeshGenerator))]
    public class PathMeshGeneratorEditor : Editor
    {
        private PathMeshGenerator path;

        private PathMeshGenerator Path
        {
            get
            {
                if (path == null)
                {
                    path = (PathMeshGenerator)target;
                }

                return path;
            }
        }

        void OnSceneGUI()
        {
            bool hasChanged = false;
            for (int i = 0; i < Path.Points.Length; i++)
            {
                Vector2 curr = Path.Points[i];
                Vector2 newPoint = Handles.PositionHandle(curr, Quaternion.identity);

                if (!hasChanged)
                {
                    hasChanged = newPoint != curr;
                }

                Path.Points[i] = newPoint;
                
            }

            if (hasChanged)
            {
                Path.GenerateMesh();
            }
        }

    }
}
*/