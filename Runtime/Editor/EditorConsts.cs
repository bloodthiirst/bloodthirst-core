using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor
{
    public class EditorConsts
    {
        private const string GLBOAL_USS_PATH = "Packages/com.bloodthirst.bloodthirst-core/Runtime/Editor/GlobalStyleSheet.uss";

        private static StyleSheet globalStyleSheet;
        public static StyleSheet GlobalStyleSheet
        {
            get
            {
                if (globalStyleSheet == null)
                {
                    globalStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(GLBOAL_USS_PATH);
                }

                return globalStyleSheet;
            }
        }
    }
}
