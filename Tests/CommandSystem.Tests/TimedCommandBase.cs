using Bloodthirst.System.CommandSystem;
using UnityEngine;

namespace Commands.Tests
{
    public class TimedCommandBase : CommandBase<TimedCommandBase>
    {
        private float currentTimer;

        private readonly float timer;

        private readonly string debug;

        public TimedCommandBase(float timer, string debug)
        {
            this.timer = timer;
            this.debug = debug;
        }

        public override void OnStart()
        {
            Debug.Log("STARTED  => " + debug);
            currentTimer = 0;
        }

        public override void OnTick(float delta)
        {
            if(currentTimer >= timer)
            {
                Success();
            }

            currentTimer += delta;
        }

        public override void OnEnd()
        {
            Debug.Log("ENDED => " + debug);
        }

    }
}
