using System;
using System.Threading;

namespace Bloodthirst.Core.ThreadProcessor
{
    public class ThreadCommandAction : ISeparateThreadCommand
    {

        private Action Func { get; set; }

        private Action OnDone { get; set; }

        private bool done;

        public bool IsDone => done;

        public ThreadCommandAction(Action Func, Action OnDone = null)
        {
            this.Func = Func;
            this.OnDone = OnDone;
            done = false;
        }

        public void Start()
        {
            Action<object> act = (state) => { RunCommand(); };
            ThreadPool.QueueUserWorkItem(new WaitCallback(act));
        }

        private void RunCommand()
        {
            Func();
            done = true;
        }

        public void ExecuteCallback()
        {
            if (!IsDone)
                return;

            OnDone?.Invoke();
        }
    }
}
