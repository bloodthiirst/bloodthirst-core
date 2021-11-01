using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BInspector
{
    [CustomEditor(typeof(BInspectorBehaviour) , true)]
    public class BInspectorBase : UnityEditor.Editor
    {
        private void OnEnable()
        {
            
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var mb = target as MonoBehaviour;

        }
    }
}
