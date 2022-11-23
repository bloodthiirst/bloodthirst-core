using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Core.UI
{
    public interface IWindowLayer
    {
        Canvas Canvas { get; }
        event Action<IWindowLayer> OnLayerFocused;
        event Action<IWindowLayer> OnLayerUnfocused;
        List<IUIWindow> UiWindows { get; }
        void Add<T>(T t) where T : IUIWindow;
        void Remove<T>(T t) where T : IUIWindow;
        IEnumerable<T> Get<T>() where T : IUIWindow;

        void OnInitialize();
        void OnDestroy();
        void Refresh();

    }
}
