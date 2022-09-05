using Bloodthirst.System.CommandSystem;
using UnityEngine;

namespace Commands.Tests
{
    public class InstantCommandBase : CommandBase<InstantCommandBase>
    {
        private readonly string debug;

        public InstantCommandBase(string debug)
        {
            this.debug = debug;
        }

        public override void OnStart()
        {
            Debug.Log("STARTED  => " + debug);
            Success();
        }

        public override void OnEnd()
        {
            Debug.Log("ENDED => " + debug);
        }
    }
}
