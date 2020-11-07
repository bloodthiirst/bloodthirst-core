using System.Collections.Generic;

namespace Bloodthirst.Core.UI
{
    public interface IStackedWindowManager
    {
        List<IUIWindow> UiWindows { get; set; }
        void Add<T>(T t) where T : IUIWindow;
        void Remove<T>(T t) where T : IUIWindow;
    }
}
