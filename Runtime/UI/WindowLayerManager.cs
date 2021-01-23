using Bloodthirst.Scripts.Core.Utils;
using System.Collections.Generic;

namespace Bloodthirst.Core.UI
{
    public static class WindowLayerManager
    {
        private static List<IWindowLayer> uiLayers;
        public static IReadOnlyList<IWindowLayer> UILayers
        {
            get
            {
                uiLayers = uiLayers.CreateIfNull();

                return uiLayers;
            }
        }

        internal static void Add(IWindowLayer stackedWindowManager)
        {
            uiLayers = uiLayers.CreateIfNull();
            uiLayers.Add(stackedWindowManager);
        }

        internal static void Remove(IWindowLayer stackedWindowManager)
        {
            uiLayers = uiLayers.CreateIfNull();
            uiLayers.Remove(stackedWindowManager);
        }
    }
}
