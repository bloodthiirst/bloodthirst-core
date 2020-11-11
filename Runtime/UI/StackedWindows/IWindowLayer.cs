using System.Collections.Generic;

namespace Bloodthirst.Core.UI
{
    public interface IWindowLayer
    {
        List<IUIWindow> UiWindows { get; }
        void PutOnTop(IUIWindow window);
        void Add<T>(T t) where T : IUIWindow;
        void Remove<T>(T t) where T : IUIWindow;
        IEnumerable<T> Get<T>() where T : IUIWindow;
        void Refresh();

    }
}
