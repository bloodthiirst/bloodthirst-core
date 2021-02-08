using Bloodthirst.Scripts.Core.Utils;
using System.Collections.Generic;

namespace Bloodthirst.Core.UI
{
    /// <summary>
    /// Class that manages the order and focus/unfocus between the multiple active instances of <see cref="IWindowLayer"/>
    /// </summary>
    public static class WindowLayerManager
    {
        /// <summary>
        /// The global offset to be added to all the <see cref="IWindowLayer.Canvas"/> instances
        /// </summary>
        static public int SortingOrderOffset { get; set; } = 100;

        private static List<IWindowLayer> uiLayers;
        private static List<IWindowLayer> _UILayers
        {
            get
            {
                uiLayers = uiLayers = uiLayers.CreateIfNull();
                return uiLayers;
            }
        }

        /// <summary>
        /// Get the list of all the window layers available
        /// </summary>
        public static IReadOnlyList<IWindowLayer> UILayers => _UILayers;

        internal static void Add(IWindowLayer stackedWindowManager)
        {
            stackedWindowManager.OnLayerFocused -= OnLayerFocused;
            stackedWindowManager.OnLayerFocused += OnLayerFocused;

            _UILayers.Add(stackedWindowManager);
        }

        internal static void Remove(IWindowLayer stackedWindowManager)
        {
            stackedWindowManager.OnLayerFocused -= OnLayerFocused;

            _UILayers.Remove(stackedWindowManager);
        }

        /// <summary>
        /// When a layer gets focused , we unfocus all the other layers and recalculate the sorting order
        /// </summary>
        /// <param name="focused"></param>
        private static void OnLayerFocused(IWindowLayer focused)
        {
            foreach (IWindowLayer layer in _UILayers)
            {
                if (layer == focused)
                {
                    continue;
                }

                foreach (IUIWindow win in layer.UiWindows)
                {
                    if (win.IsOpen || win.RequestOpen)
                    {
                        win.RequestUnfocus = true;
                    }
                }

                layer.Refresh();
            }

            _UILayers.Remove(focused);
            _UILayers.Add(focused);

            for (int i = 0; i < _UILayers.Count; i++)
            {
                _UILayers[i].Canvas.sortingOrder = SortingOrderOffset + i;
            }
        }

    }
}
