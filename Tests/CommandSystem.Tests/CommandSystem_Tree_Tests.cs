using System.Collections;
using Bloodthirst.System.CommandSystem;
using Commands.Tests;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CommandSystem_Tree_Tests
{
    [UnityTest]
    public IEnumerator Test_Sub_Command_Should_Branch_In_Correct_Order_CommandBatchTree()
    {
        CommandManager commandManager = new CommandManager();

        BasicTreeCommand tree = new BasicTreeCommand(true);

        commandManager.AppendCommand(this, tree , true);

        CommandNodeBuilder root1 = tree
                        .Append(new InstantCommandBase("1 : depth 0"), true)
                        .Append(new InstantCommandBase("1 : depth 1"), true)
                        .Append(new InstantCommandBase("1 : depth 2"), true);

        CommandNodeBuilder root2 = tree
                        .Append(new InstantCommandBase("2 : depth 0"), true)
                        .Append(new InstantCommandBase("2 : depth 1"))
                        .Append(new InstantCommandBase("2 : depth 2"));

        while (!tree.IsDone())
        {
            commandManager.Tick(Time.deltaTime);
            yield return null;
        }

        Assert.AreEqual(COMMAND_STATE.SUCCESS, tree.CommandState);
    }
}