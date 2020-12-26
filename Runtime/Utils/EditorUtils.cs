#if UNITY_EDITOR
using System.Linq;
using UnityEditor;

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
    }
}
#endif
