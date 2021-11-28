using Bloodthirst.JsonUnityObject;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Bloodthirst.Editor.BInspector
{
    public interface IValueDrawerValidator
    {
        void Initialize();
        int Order { get; }

        bool CanDraw(Type type);

        IValueDrawer GetValueDrawer();
    }
}
