using System;
using System.Threading;

namespace Bloodthirst.Core.ThreadProcessor
{
    public class ThreadCommandFunc<T> : ISeparateThreadCommand
    {
        private T Result { get; set; }

        private Func<T> Func { get; set; }

        private Action<T> OnDone { get; set; }

        private bool done;

        public bool IsDone => done;

        public ThreadCommandFunc(Func<T> Func, Action<T> OnDone)
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
            Result = Func();
            done = true;
        }

        public void ExecuteCallback()
        {
            if (!IsDone)
                return;

            OnDone?.Invoke(Result);
        }
    }
}
