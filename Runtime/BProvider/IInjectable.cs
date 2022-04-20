namespace Bloodthirst.Core.BProvider
{

    internal interface IInjectable
    {
        object GetInstance(BProvider provider);
    }
}