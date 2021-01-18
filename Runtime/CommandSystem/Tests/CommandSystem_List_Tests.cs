using System.Collections;
using System.Collections.Generic;
using Bloodthirst.System.CommandSystem;
using Commands.Tests;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CommandSystem_List_Tests
{
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator Test_Sub_Command_Fail_Should_Interrupt_Main_CommandBatchList()
    {
        CommandManager commandManager = new CommandManager();

        CommandBatchList queue = commandManager.AppendBatch<CommandBatchList>(this);

        queue
            .Append(new TimedCommandBase(1, "1"))
            .Append(new TimedCommandBase(1, "2"))
            .Append(new TimedCommandBase(1, "3"))
            .Append(new AlwaysFailCommandBase(), true)
            .Append(new TimedCommandBase(1, "4"));

        while (queue.BatchState == BATCH_STATE.EXECUTING)
        {
            commandManager.Tick(Time.deltaTime);
            yield return null;
        }

        Assert.AreEqual(BATCH_STATE.INTERRUPTED, queue.BatchState);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator Test_Sub_ListCommand_Fail_Should_Interrupt_Main_CommandBatchList()
    {
        CommandManager commandManager = new CommandManager();

        CommandBatchQueue queue = commandManager.AppendBatch<CommandBatchQueue>(this);

        TestListCommand testListCommand = new TestListCommand(commandManager, true);

        testListCommand
            .AddToList(new TimedCommandBase(1, "1"))
            .AddToList(new TimedCommandBase(1, "2"))
            .AddToList(new TimedCommandBase(1, "3"))
            .AddToList(new AlwaysFailCommandBase(), true)
            .AddToList(new TimedCommandBase(1, "4"));

        queue
            .Append(new TimedCommandBase(1, "1"))
            .Append(new TimedCommandBase(1, "2"))
            .Append(new TimedCommandBase(1, "3"))
            .Append(testListCommand, true)
            .Append(new TimedCommandBase(1, "4"));

        while (queue.BatchState == BATCH_STATE.EXECUTING)
        {
            commandManager.Tick(Time.deltaTime);
            yield return null;
        }

        Assert.AreEqual(BATCH_STATE.INTERRUPTED, queue.BatchState);
    }
}
