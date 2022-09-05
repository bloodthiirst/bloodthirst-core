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

        BasicListCommand list = new BasicListCommand(true);
        
        list
            .Add(new TimedCommandBase(1, "1"), true)
            .Add(new TimedCommandBase(1, "2"), true)
            .Add(new TimedCommandBase(1, "3"), true)
            .Add(new AlwaysFailCommandBase(), true)
            .Add(new TimedCommandBase(1, "4"), true);

        commandManager.AppendCommand(this, list, true);

        while (!list.IsDone())
        {
            commandManager.Tick(Time.deltaTime);
            yield return null;
        }

        Assert.AreEqual(COMMAND_STATE.FAILED, list.CommandState);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator Test_Sub_ListCommand_Fail_Should_Also_Fail_Main_CommandBatchList()
    {
        CommandManager commandManager = new CommandManager();

        BasicListCommand parentList = new BasicListCommand(true);
        BasicListCommand subList = new BasicListCommand(true);

        subList.Add(new AlwaysFailCommandBase(), true);

        parentList.Add(subList, true);
        
        commandManager.AppendCommand(this, parentList, true);

        while (!parentList.IsDone())
        {
            commandManager.Tick(Time.deltaTime);
            yield return null;
        }

        Assert.AreEqual(COMMAND_STATE.FAILED, parentList.CommandState);
    }
}