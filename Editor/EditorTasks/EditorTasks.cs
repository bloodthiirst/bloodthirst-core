using System.Collections.Generic;
using UnityEngine;
using Bloodthirst.System.CommandSystem;
using UnityEditor;
using Sirenix.Serialization;
using System.IO;
using System.Text;
using Bloodthirst.Editor.Commands;
using System;

namespace Bloodthirst.Editor
{
    public class EditorTasksData
    {
        public List<ICommandBase> commands;
    }

    public static class EditorTasks
    {
        private static string DATA_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/EditorTasks/EditorTasks.json";

        private static string jsonAsset;
        private static string JsonAsset
        {
            get
            {
                if (jsonAsset == null)
                {
                    string path = Path.GetFullPath(DATA_PATH);

                    if (!File.Exists(path))
                    {
                        StreamWriter sw = File.CreateText(path);
                        sw.Dispose();

                        AssetDatabase.Refresh();

                        jsonAsset = string.Empty;
                    }
                    else
                    {
                        jsonAsset = File.ReadAllText(path);
                    }
                }

                return jsonAsset;
            }
        }

        private static EditorTasksData tasksData;
        private static BasicQueueCommand tasksQueue;

        private static EditorTasksData Load()
        {
            string jsonStr = JsonAsset;
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonStr);

            EditorTasksData res = Sirenix.Serialization.SerializationUtility.DeserializeValue<EditorTasksData>(jsonBytes, DataFormat.JSON);

            return res;
        }

        private static void Save(EditorTasksData data)
        {
            string path = Path.GetFullPath(DATA_PATH);
            byte[] jsonBytes = Sirenix.Serialization.SerializationUtility.SerializeValue(data, DataFormat.JSON);
            string jsonStr = Encoding.UTF8.GetString(jsonBytes);

            File.WriteAllText(path, jsonStr);
            AssetDatabase.Refresh();
        }


        [InitializeOnLoadMethod]
        public static void Init()
        {
            tasksData = Load();

            if (tasksData == null)
            {
                tasksData = new EditorTasksData() { commands = new List<ICommandBase>() };
            }

            tasksQueue = new BasicQueueCommand(false);

            CommandManagerEditor.AppendCommand(null, tasksQueue, false, 0);

            foreach (ICommandBase cmd in tasksData.commands)
            {
                tasksQueue.Enqueue(cmd);

                cmd.OnCommandStart += HandleStarted;
            }

            AssemblyReloadEvents.beforeAssemblyReload += HandleBeforeReload;
        }

        private static void HandleBeforeReload()
        {
            Save(tasksData);
        }

        public static void Add(Action act)
        {
            ActionCommand cmd = new ActionCommand(act);

            tasksData.commands.Add(cmd);
            tasksQueue.Enqueue(cmd);

            cmd.OnCommandEnd += HandleStarted;
        }

        private static void HandleStarted(ICommandBase c)
        {
            tasksData.commands.Remove(c);
        }
    }

    public class ActionCommand : CommandBase<ActionCommand>
    {
        [OdinSerialize]
        private readonly Action act;

        public ActionCommand(Action act)
        {
            this.act = act;
        }

        public override void OnTick(float delta)
        {
            act?.Invoke();
            Success();
        }
    }
}
