﻿using Bloodthirst.Editor.BInspector;
using Bloodthirst.Runtime.BRecorder;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BRecorder
{
    public class BRecorderCommandInfoUI : VisualElement
    {
        public IBRecorderCommand Command { get; private set; }

        public BRecorderCommandInfoUI(IBRecorderCommand command)
        {
            Command = command;

            IBInspectorDrawer drawer = BInspectorProvider.DefaultInspector;

            VisualElement inspector = drawer.CreateInspectorGUI(Command);
            Add(inspector);

            AddToClassList("cmd-main");
            style.position = new StyleEnum<Position>(Position.Absolute);
        }

    }
}
