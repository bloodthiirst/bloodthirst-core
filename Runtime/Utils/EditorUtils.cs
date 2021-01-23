#if UNITY_EDITOR
using Bloodthirst.Scripts.Utils;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Bloodthirst.Core.Utils
{
    /// <summary>
    /// an extension class containing helper methods for types / reflection
    /// </summary>
    public static class EditorUtils
    {
        /// <summary>
        /// <para>Create a folder based on path</para>
        /// <para>example : "Assets/Resources/Foo/Bar"</para>
        /// </summary>
        /// <param name="folderPath"></param>
        public static void CreateFoldersFromPath(string folderPath)
        {
            string[] folders = folderPath.Split('/');

            for (int i = 0; i < folders.Length; i++)
            {
                string folderToCheck = string.Join("/", folders.Take(i + 1));
                string parentFolder = string.Join("/", folders.Take(i));

                if (!AssetDatabase.IsValidFolder(folderToCheck))
                {
                    AssetDatabase.CreateFolder(parentFolder, folders[i]);
                }
            }
        }

        public static void HandlesDrawCubeOutline(Vector3 center, Vector3 size)
        {
            Vector3 halfSize = size * 0.5f;

            //center += halfSize;

            Vector3[] pointsA = new Vector3[5];
            // side A
            pointsA[0] = center + halfSize.MulVector(1, 1, 1);
            pointsA[1] = center + halfSize.MulVector(1, -1, 1);
            pointsA[2] = center + halfSize.MulVector(-1, -1, 1);
            pointsA[3] = center + halfSize.MulVector(-1, 1, 1);
            pointsA[4] = center + halfSize.MulVector(1, 1, 1);

            Handles.DrawAAPolyLine(pointsA);

            Vector3[] pointsB = new Vector3[5];
            // side B
            pointsB[0] = center + halfSize.MulVector(1, 1, -1);
            pointsB[1] = center + halfSize.MulVector(1, -1, -1);
            pointsB[2] = center + halfSize.MulVector(-1, -1, -1);
            pointsB[3] = center + halfSize.MulVector(-1, 1, -1);
            pointsB[4] = center + halfSize.MulVector(1, 1, -1);

            Handles.DrawAAPolyLine(pointsB);

            Handles.DrawLine(pointsA[0], pointsB[0]);
            Handles.DrawLine(pointsA[1], pointsB[1]);
            Handles.DrawLine(pointsA[2], pointsB[2]);
            Handles.DrawLine(pointsA[3], pointsB[3]);

        }

        public static void HandlesDrawCube(Vector3 center, Vector3 size)
        {
            Vector3 halfSize = size * 0.5f;

            //center += halfSize;

            Vector3[] points = new Vector3[4];
            // side A
            points[0] = center + halfSize.MulVector(1, 1, 1);
            points[1] = center + halfSize.MulVector(1, -1, 1);
            points[2] = center + halfSize.MulVector(-1, -1, 1);
            points[3] = center + halfSize.MulVector(-1, 1, 1);

            Handles.DrawAAConvexPolygon(points);

            // side B
            points[0] = center + halfSize.MulVector(1, 1, -1);
            points[1] = center + halfSize.MulVector(1, -1, -1);
            points[2] = center + halfSize.MulVector(-1, -1, -1);
            points[3] = center + halfSize.MulVector(-1, 1, -1);

            Handles.DrawAAConvexPolygon(points);

            // side C
            points[0] = center + halfSize.MulVector(1, 1, 1);
            points[1] = center + halfSize.MulVector(1, 1, -1);
            points[2] = center + halfSize.MulVector(-1, 1, -1);
            points[3] = center + halfSize.MulVector(-1, 1, 1);

            Handles.DrawAAConvexPolygon(points);
            // side D
            points[0] = center + halfSize.MulVector(1, -1, 1);
            points[1] = center + halfSize.MulVector(1, -1, -1);
            points[2] = center + halfSize.MulVector(-1, -1, -1);
            points[3] = center + halfSize.MulVector(-1, -1, 1);
            Handles.DrawAAConvexPolygon(points);
            // side E
            points[0] = center + halfSize.MulVector(1, 1, 1);
            points[1] = center + halfSize.MulVector(1, 1, -1);
            points[2] = center + halfSize.MulVector(1, -1, -1);
            points[3] = center + halfSize.MulVector(1, -1, 1);
            Handles.DrawAAConvexPolygon(points);
            // side F
            points[0] = center + halfSize.MulVector(-1, 1, 1);
            points[1] = center + halfSize.MulVector(-1, 1, -1);
            points[2] = center + halfSize.MulVector(-1, -1, -1);
            points[3] = center + halfSize.MulVector(-1, -1, 1);

            Handles.DrawAAConvexPolygon(points);
        }
    }
}
#endif
