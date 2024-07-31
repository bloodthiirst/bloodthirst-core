using System.Collections;

namespace Bloodthirst.UI
{
    /// <summary>
    /// Interface to add to scripts attached to a <see cref="WindowUIBase"/> and that want to override the opening/closing animation
    /// </summary>
    public interface IWindowUIEffect
    {
        IEnumerator OpenCrt(IWindowUI window);
        IEnumerator CloseCrt(IWindowUI window);
        void OpenImmidiate(IWindowUI window);
        void CloseImmidiate(IWindowUI window);
    }
}
