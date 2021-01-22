using Bloodthirst.Core.PersistantAsset;

namespace Bloodthirst.System.ContextSystem
{

    public abstract class ContextInstance<T> : SingletonScriptableObject<T>, IContextInstance where T : ContextInstance<T>
    {
    }
}
