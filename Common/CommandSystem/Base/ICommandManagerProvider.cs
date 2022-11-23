namespace Bloodthirst.System.CommandSystem
{
    public interface ICommandManagerProvider
    {
        void Initialize();

        CommandManager Get();

    }
}
