using Bloodthirst.System.CommandSystem;
using UnityEngine;

namespace Assets.Scripts.SocketLayer.Commands
{
    public class WaitSencondsCommand : CommandBase<WaitSencondsCommand>
    {
        /// <summary>
        /// Time in milliseconds
        /// </summary>
        private readonly float time;

        /// <summary>
        /// current time spent
        /// </summary>
        private float currentTimer;

        /// <summary>
        /// Time in milliseconds
        /// </summary>
        /// <param name="time"></param>
        public WaitSencondsCommand(float timeInSeconds)
        {
            this.time = timeInSeconds;
        }

        public override void OnStart()
        {
            Debug.Log("Started timer of " + time + " seconds");
            this.currentTimer = 0;
        }


        public override void OnTick(float delta)
        {
            if (currentTimer >= time)
            {
                CommandState = COMMAND_STATE.SUCCESS;
                Success();
                return;
            }
            Debug.Log("current timer value : " + currentTimer);

            currentTimer += delta;
        }

        public override void OnEnd()
        {
            Debug.Log("Timer ended");
        }

    }
}
