namespace Bloodthirst.System.CommandSystem
{
    public interface IResult<TResult>
    {
        bool IsReady { get; }
        TResult Result { get; }
    }
}