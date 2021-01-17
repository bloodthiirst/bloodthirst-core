using Bloodthirst.System.CommandSystem;
using UnityEngine;

namespace Commands.Tests
{
    public class AlwaysFailCommandBase : CommandBase<AlwaysFailCommandBase>
    {
        public override void OnStart()
        {
            Debug.Log("FAILED");
            Fail();
        }
    }
}
