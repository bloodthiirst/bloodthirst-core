using System.Reflection;

namespace Bloodthirst.Editor.BNodeTree
{
    public interface IBindableUIFactory
    {
        bool CanBind(MemberInfo memberInfo);
        IBindableUI CreateUI();
    }
}
