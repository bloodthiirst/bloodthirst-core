using Bloodthirst.JsonUnityObject;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BInspector
{
    [CustomEditor(typeof(JsonMonoBehaviour), true)]
    public class JsonMonoBehaviourEditor : UnityEditor.Editor
    {

        private void OnEnable()
        {

        }

        public override VisualElement CreateInspectorGUI()
        {
            IBInspectorDrawer editorCreator = BInspectorProvider.Get(target);

            if(editorCreator == null)
            {
                editorCreator = BInspectorProvider.DefaultInspector;
            }
            
            return editorCreator.CreateInspectorGUI(target);
        }
    }
}
