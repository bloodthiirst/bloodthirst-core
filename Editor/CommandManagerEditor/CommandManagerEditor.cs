#if UNITY_EDITOR
using Bloodthirst.System.CommandSystem;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Bloodthirst.Editor.Commands
{
    [InitializeOnLoad]
    public class CommandManagerEditor
    {
        private static CommandManager commandManager;

        private static double lastTime;
        static CommandManagerEditor()
        {
            commandManager = new CommandManager();

            lastTime = EditorApplication.timeSinceStartup;

            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;
        }

        private static void OnEditorUpdate()
        {
            double delta = EditorApplication.timeSinceStartup - lastTime;
            
            commandManager.Tick((float) delta);

            lastTime = EditorApplication.timeSinceStartup;
        }

        public static TBatch AppendBatch<TBatch>(object owner, bool removeWhenDone, int updateOrder) where TBatch : ICommandBatch , new()
        {
           return commandManager.AppendBatch<TBatch>(owner, removeWhenDone, updateOrder);
        }

        [Button]
        public static void TestCommand()
        {
            CommandBatchList b = AppendBatch<CommandBatchList>(new object(), true, 0);
            b.Append(new TimedCommandBase(1f, "Editor Cmd Test"));
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