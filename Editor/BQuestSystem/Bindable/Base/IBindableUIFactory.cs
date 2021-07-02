using System.Reflection;

namespace Bloodthirst.System.Quest.Editor
{
    public interface IBindableUIFactory
    {
        bool CanBind(MemberInfo memberInfo);
        IBindableUI CreateUI();
    }
}
