using System;

namespace Bloodthirst.Core.ThreadProcessor
{
    public class ThreadCommandMainAction : IMainThreadCommand
    {

        private Action Func { get; set; }

        private Action OnDone { get; set; }


        public ThreadCommandMainAction(Action Func, Action OnDone = null)
        {
            this.Func = Func;
            this.OnDone = OnDone;
        }


        public void ExecuteCallback()
        {
            Func?.Invoke();

            OnDone?.Invoke();
        }
    }
}
