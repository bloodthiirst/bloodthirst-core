using Bloodthirst.System.CommandSystem;
using UnityEngine;

namespace Assets.Scripts.SocketLayer.Commands
{
    public class WaitFramesCommand : CommandBase<WaitSencondsCommand>
    {
        /// <summary>
        /// Time in milliseconds
        /// </summary>
        private readonly int frames;

        /// <summary>
        /// current time spent
        /// </summary>
        private float currentFrame;

        /// <summary>
        /// Time in milliseconds
        /// </summary>
        /// <param name="time"></param>
        public WaitFramesCommand(int frames)
        {
            this.frames = frames;
        }

        public override void OnStart()
        {
            this.currentFrame = 0;
        }


        public override void OnTick(float delta)
        {
            if (currentFrame == frames)
            {
                CommandState = COMMAND_STATE.SUCCESS;
                IsDone = true;
                return;
            }

            currentFrame++;
        }

        public override void OnEnd()
        {
            Debug.Log("Timer ended");
        }

    }
}
