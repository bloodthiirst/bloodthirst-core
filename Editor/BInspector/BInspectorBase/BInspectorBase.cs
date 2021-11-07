using Bloodthirst.JsonUnityObject;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BInspector
{
    [CustomEditor(typeof(JsonMonoBehaviour) , true)]
    public class JsonMonoBehaviourEdtior : UnityEditor.Editor
    {
        private void OnEnable()
        {
            
        }

        public override VisualElement CreateInspectorGUI()
        {
            return base.CreateInspectorGUI();
        }
    }
}
