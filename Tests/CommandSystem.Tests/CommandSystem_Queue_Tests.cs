using System.Collections;
using System.Collections.Generic;
using Bloodthirst.System.CommandSystem;
using Commands.Tests;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CommandSystem_Queue_Tests
{
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator Test_Sub_Command_Fail_Should_Interrupt_Main_CommandBatchQueue()
    {
        CommandManager commandManager = new CommandManager();

        BasicQueueCommand queue = new BasicQueueCommand(true);
        
        commandManager.AppendCommand(this , queue , true );

        queue
            .Enqueue(new TimedCommandBase(1, "1") , true)
            .Enqueue(new TimedCommandBase(1, "2") , true )
            .Enqueue(new TimedCommandBase(1, "3") , true )
            .Enqueue(new AlwaysFailCommandBase(), true )
            .Enqueue(new TimedCommandBase(1, "4") , true);

        while (!queue.IsDone())
        {
            commandManager.Tick(Time.deltaTime);
            yield return null;
        }

        Assert.AreEqual(COMMAND_STATE.FAILED, queue.CommandState);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator Test_Sub_QueueCommand_Fail_Should_Interrupt_Main_CommandBatchQueue()
    {
        CommandManager commandManager = new CommandManager();

        BasicQueueCommand parentQueue = new BasicQueueCommand(true);

        BasicQueueCommand subQueue = new BasicQueueCommand(true);

        commandManager.AppendCommand(this, parentQueue, true);

        subQueue
            .Enqueue(new TimedCommandBase(1, "1"), true)
            .Enqueue(new TimedCommandBase(1, "2"), true)
            .Enqueue(new TimedCommandBase(1, "3"), true)
            .Enqueue(new AlwaysFailCommandBase(), true)
            .Enqueue(new TimedCommandBase(1, "4"), true);

        parentQueue
            .Enqueue(new TimedCommandBase(1, "1"), true)
            .Enqueue(new TimedCommandBase(1, "2"), true)
            .Enqueue(new TimedCommandBase(1, "3"), true)
            .Enqueue(subQueue, true)
            .Enqueue(new TimedCommandBase(1, "4"), true);

        while (!parentQueue.IsDone())
        {
            commandManager.Tick(Time.deltaTime);
            yield return null;
        }

        Assert.AreEqual(COMMAND_STATE.FAILED, parentQueue.CommandState);
    }
}
