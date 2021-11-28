using Bloodthirst.JsonUnityObject;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Bloodthirst.Editor.BInspector
{
    public interface IBInspectorValidator
    {
        void Initialize();
        int Order { get; }

        bool CanInspect(Type type , object instance);

        IBInspectorDrawer GetDrawer();
    }
}
