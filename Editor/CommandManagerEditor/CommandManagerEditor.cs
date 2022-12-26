#if UNITY_EDITOR
using Bloodthirst.System.CommandSystem;
#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Bloodthirst.Editor.Commands
{
    [InitializeOnLoad]
    public class CommandManagerEditor
    {
        private static CommandManager commandManager;

        private static double lastTime;

        private static BasicListCommand globalList;

        static CommandManagerEditor()
        {
            if (!EditorConsts.ON_ASSEMBLY_RELOAD_COMMAND_MANAGER_EDITOR)
                return;

            commandManager = new CommandManager();

            globalList = new BasicListCommand(false);
            commandManager.AppendCommand(null, globalList , false);

            lastTime = EditorApplication.timeSinceStartup;

            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;
        }

        private static void OnEditorUpdate()
        {
            double delta = EditorApplication.timeSinceStartup - lastTime;

            commandManager.Tick((float)delta);

            lastTime = EditorApplication.timeSinceStartup;
        }

        public static void AppendCommand(object owner, ICommandBase cmd, bool removeWhenDone, int updateOrder)
        {
            Assert.IsTrue(EditorConsts.ON_ASSEMBLY_RELOAD_COMMAND_MANAGER_EDITOR);

            commandManager.AppendCommand(owner, cmd, removeWhenDone, updateOrder);
        }

        public static void Run(ICommandBase cmd)
        {
            Assert.IsTrue(EditorConsts.ON_ASSEMBLY_RELOAD_COMMAND_MANAGER_EDITOR);

            globalList.Add(cmd , true );
        }

        public static void RunInstant(ICommandInstant cmd)
        {
            Assert.IsTrue(EditorConsts.ON_ASSEMBLY_RELOAD_COMMAND_MANAGER_EDITOR);

            cmd.Execute();
        }

        public static T RunInstant<T>(ICommandInstant<T> cmd)
        {
            Assert.IsTrue(EditorConsts.ON_ASSEMBLY_RELOAD_COMMAND_MANAGER_EDITOR);

            return cmd.GetResult();
        }

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
                if (currentTimer >= timer)
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
}
#endif