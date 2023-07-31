#if ODIN_INSPECTOR &&  UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Bloodthirst.System.ContextSystem
{
    public class CommandManagerPropertyProcessor<T> : OdinPropertyProcessor<T> where T : ContextSystemManager
    {
        private static Attribute[] buttonAttrs = new Attribute[]
        {
            new ButtonAttribute(ButtonSizes.Gigantic , ButtonStyle.Box)
        };

        public override void ProcessMemberProperties(List<InspectorPropertyInfo> propertyInfos)
        {
            Delegate initializeDel = typeof(ContextSystemManagerEditor)
                .GetMethod(nameof(ContextSystemManagerEditor.Initialize), BindingFlags.NonPublic | BindingFlags.Static)
                .CreateDelegate(typeof(Action));

            InspectorPropertyInfo initializeButton = InspectorPropertyInfo.CreateForDelegate("Initialize", 0, typeof(ContextSystemManagerEditor), initializeDel, buttonAttrs);
            propertyInfos.Add(initializeButton);
        }
    }
}
#endif
