using Bloodthirst.Core.Utils;
using Bloodthirst.Editor.BInspector;
using Bloodthirst.Runtime.BRecorder;
using Bloodthirst.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BRecorder
{
    [InitializeOnLoad]
    public class BRecorderEditor : EditorWindow
    {
        private const string PATH_UXML = "Packages/com.bloodthirst.bloodthirst-core/Editor/BRecorder/UI/BRecorderEditor.uxml";
        private const string PATH_USS = "Packages/com.bloodthirst.bloodthirst-core/Editor/BRecorder/UI/BRecorderEditor.uss";

        private const float PIXEL_PER_SECOND = 40f;

        private const float TIMELINE_HEIGHT = 200;

        [MenuItem("Bloodthirst Tools/BRecorder")]
        public static void OpenEditor()
        {
            BRecorderEditor wnd = GetWindow<BRecorderEditor>();
            wnd.minSize = new Vector2(500, 500);
            wnd.titleContent = new GUIContent("BRecorder");
        }

        static BRecorderEditor()
        {
            BRecorderRuntime.OnStateChanged -= HandleOpenEditorIfNeeded;
            BRecorderRuntime.OnStateChanged += HandleOpenEditorIfNeeded;
        }

        private static void HandleOpenEditorIfNeeded(BRecorderRuntime.RECORDER_STATE oldState, BRecorderRuntime.RECORDER_STATE newState)
        {
            if (newState == BRecorderRuntime.RECORDER_STATE.RECORDING)
            {
                if (HasOpenInstances<BRecorderEditor>())
                {
                    BRecorderEditor wnd = GetWindow<BRecorderEditor>();
                }
                else
                {
                    BRecorderEditor wnd = GetWindow<BRecorderEditor>();
                    wnd.minSize = new Vector2(500, 500);
                    wnd.titleContent = new GUIContent("BRecorder");
                }
            }
        }



        internal VisualElement Root { get; set; }
        internal Label GameTimer => Root.Q<Label>(nameof(GameTimer));
        internal VisualElement RecordBtn => Root.Q<VisualElement>(nameof(RecordBtn));
        internal VisualElement PlayBtn => Root.Q<VisualElement>(nameof(PlayBtn));
        internal VisualElement StopBtn => Root.Q<VisualElement>(nameof(StopBtn));
        internal VisualElement PauseBtn => Root.Q<VisualElement>(nameof(PauseBtn));
        internal VisualElement Timeline => Root.Q<VisualElement>(nameof(Timeline));
        internal VisualElement TimeAxis => Root.Q<VisualElement>(nameof(TimeAxis));
        internal VisualElement CommandsContainer => Root.Q<VisualElement>(nameof(CommandsContainer));
        internal ScrollView TimelineContainer => Root.Q<ScrollView>(nameof(TimelineContainer));

        private List<BRecorderCommandCursorUI> Cursors { get; set; }
        private List<BRecorderCommandInfoUI> Infos { get; set; }

        private Vector2 horizontalZoomMinMax;

        private Vector2 horizontalScrollMinMax;

        private float horizontalScrollValue;
        internal float HorizontalScrollValue
        {
            get => horizontalScrollValue;
            set
            {
                horizontalScrollValue = Mathf.Clamp(value, horizontalScrollMinMax.x, horizontalScrollMinMax.y);
                RepaintViewport();
                RepaintTimeAxis();
                RepaintTimline();
            }
        }

        internal Vector2 worldMousePostion;

        private float currentHorizontalZoom;
        internal float CurrentHorizontalZoom
        {
            get => currentHorizontalZoom;
            set
            {
                float old = currentHorizontalZoom;
                currentHorizontalZoom = Mathf.Clamp(value, horizontalZoomMinMax.x, horizontalZoomMinMax.y);
                RepaintViewport();
                RepaintTimeAxis();
                RepaintTimline();
            }
        }


        private List<BRecorderActionBase> Actions { get; set; }
        public BEventSystem.BEventSystem<BRecorderEventBase> EventSystem { get; private set; }

        private void OnEnable()
        {
            EditorApplication.update -= HandleOnUpdate;
            EditorApplication.update += HandleOnUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= HandleOnUpdate;
        }

        private void HandleOnUpdate()
        {
            if(!Application.isPlaying)
            {
                GameTimer.text = "In Editor";
                return;
            }

            TimeSpan t = new TimeSpan(0, 0, (int) Time.realtimeSinceStartup);

            DateTime asDate = new DateTime(t.Ticks);

            GameTimer.text = asDate.ToString("mm:ss");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            Root = rootVisualElement;

            // Import
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(PATH_USS);
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(PATH_UXML);
            visualTree.CloneTree(Root);
            Root.styleSheets.Add(EditorConsts.GlobalStyleSheet);
            Root.styleSheets.Add(styleSheet);

            Initialize();
            InitUI();
            ListenUI();
        }

        private void Initialize()
        {
            EventSystem = new BEventSystem.BEventSystem<BRecorderEventBase>();
            Actions = new List<BRecorderActionBase>();

            List<Type> actionTypes = TypeUtils.AllTypes
                .Where(t => t.IsClass)
                .Where(t => !t.IsAbstract)
                .Where(t => TypeUtils.IsSubTypeOf(t, typeof(BRecorderActionBase)))
                .ToList();

            foreach (Type t in actionTypes)
            {
                BRecorderActionBase action = (BRecorderActionBase)Activator.CreateInstance(t, args: this);
                action.Initialize();
                Actions.Add(action);
            }
        }

        private void InitUI()
        {
            Cursors = new List<BRecorderCommandCursorUI>();
            Infos = new List<BRecorderCommandInfoUI>();

            TimelineContainer.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            TimelineContainer.horizontalScrollerVisibility = ScrollerVisibility.AlwaysVisible;

            horizontalZoomMinMax = new Vector2(0.1f, 10f);
            CurrentHorizontalZoom = 1;
            EventSystem.Trigger(new OnTimelineSessionChanged(this, null, BRecorderRuntime.CurrentSession));
            EventSystem.Trigger(new OnTimelineStateChanged(this, BRecorderRuntime.RECORDER_STATE.IDLE, BRecorderRuntime.RecordingState));

            RepaintViewport();
            RepaintTimeAxis();
            RepaintTimline();
        }

        private void ListenUI()
        {
            BRecorderRuntime.OnStateChanged += HandleStateChanged;
            BRecorderRuntime.OnSessionChanged += HandleSessionChanged;

            Root.RegisterCallback<GeometryChangedEvent>(HandleWindowSizeChanged);

            PlayBtn.RegisterCallback<ClickEvent>(HandlePlayClicked);
            RecordBtn.RegisterCallback<ClickEvent>(HandleRecordClicked);
            StopBtn.RegisterCallback<ClickEvent>(HandleStopClicked);
            PauseBtn.RegisterCallback<ClickEvent>(HandlePauseClicked);

            Root.RegisterCallback<MouseMoveEvent>(HandleEditorMouseMove);
            Root.RegisterCallback<KeyDownEvent>(HandleEditorKeydown);
            Timeline.RegisterCallback<WheelEvent>(HandleScrollWheel);
            Timeline.RegisterCallback<MouseDownEvent>(HandleMouseDown);
            Timeline.RegisterCallback<MouseMoveEvent>(HandleMouseMove);
            Timeline.RegisterCallback<MouseUpEvent>(HandleMouseUp);
            Timeline.RegisterCallback<MouseLeaveEvent>(HandleMouseLeave);
        }


        private void HandleWindowSizeChanged(GeometryChangedEvent evt)
        {
            RepaintViewport();
            RepaintTimeAxis();
            RepaintTimline();
        }

        private void HandleSessionChanged(BRecorderSession oldSession, BRecorderSession newSession)
        {
            EventSystem.Trigger(new OnTimelineSessionChanged(this, oldSession, newSession));
        }

        private void HandleStateChanged(BRecorderRuntime.RECORDER_STATE oldState, BRecorderRuntime.RECORDER_STATE newState)
        {
            EventSystem.Trigger(new OnTimelineStateChanged(this, oldState, newState));
        }

        internal void RepaintViewport()
        {
            /*
            // height
           
            Timeline.parent.style.height = neededHeight;

            float heightInStyle = neededHeight
                - Timeline.resolvedStyle.paddingLeft
                - Timeline.resolvedStyle.paddingRight
                - Timeline.resolvedStyle.borderLeftWidth
                - Timeline.resolvedStyle.borderRightWidth;

            Timeline.style.height = heightInStyle;
            */

            Timeline.style.height = TIMELINE_HEIGHT;

            // width
            float lastCommandTime = 0;

            if (BRecorderRuntime.CurrentSession != null && BRecorderRuntime.CurrentSession.Commands.Count > 0)
            {
                lastCommandTime = BRecorderRuntime.CurrentSession.Commands.Last().GameTime;
            }

            float defaultWidth = TimelineContainer.contentViewport.resolvedStyle.width;
            float contentWidth = lastCommandTime * PIXEL_PER_SECOND * CurrentHorizontalZoom;
            float neededWidth = Mathf.Max(defaultWidth, contentWidth);

            float widthInStyle = neededWidth;
            /*
                - Timeline.resolvedStyle.paddingLeft
                - Timeline.resolvedStyle.paddingRight
                - Timeline.resolvedStyle.borderLeftWidth
                - Timeline.resolvedStyle.borderRightWidth;
            */

            Timeline.style.width = widthInStyle;
        }

        internal void RepaintTimeAxis()
        {
            TimeAxis.Clear();

            // pixel per one second
            float PixelsPerStep = PIXEL_PER_SECOND * CurrentHorizontalZoom;

            // maximum seconds we can show on the axis
            float totalStepsOnAxis = TimeAxis.resolvedStyle.width / PixelsPerStep;

            float labelSize = 40f;

            // maximum amount of labels to show in the axis
            float totalLabelsToShow = TimeAxis.resolvedStyle.width / labelSize;

            int secondPerStep = 1;

            float stepCount = totalStepsOnAxis;

            if (totalStepsOnAxis > totalLabelsToShow)
            {
                while (stepCount > totalLabelsToShow)
                {
                    secondPerStep++;
                    stepCount = totalStepsOnAxis / secondPerStep;
                }
            }

            for (int i = 0; i < stepCount; i++)
            {
                int seconds = i * secondPerStep;
                
                TimeSpan t = new TimeSpan(0, 0, seconds);

                Label timeLabel = new Label();
                timeLabel.name = $"sec-{seconds}";
                timeLabel.text = $"{seconds}";
                timeLabel.style.width = labelSize;

                TimeAxis.Add(timeLabel);

                timeLabel.style.position = new StyleEnum<Position>(Position.Absolute);
                timeLabel.style.left = (i * secondPerStep * PixelsPerStep);
            }
        }

        internal void RepaintTimline()
        {
            CommandsContainer.Clear();

            Cursors.Clear();
            Infos.Clear();

            if (BRecorderRuntime.CurrentSession == null)
                return;

            for (int i = 0; i < BRecorderRuntime.CurrentSession.Commands.Count; i++)
            {
                IBRecorderCommand cmd = BRecorderRuntime.CurrentSession.Commands[i];

                //BRecorderCommandInfoUI cmdInfo = new BRecorderCommandInfoUI(cmd);
                BRecorderCommandCursorUI cmdCursor = new BRecorderCommandCursorUI(cmd);

                CommandsContainer.Add(cmdCursor);
                Cursors.Add(cmdCursor);

                //Infos.Add(cmdInfo);
                //CommandsContainer.Add(cmdInfo);

                //cmdInfo.style.left = cmd.GameTime * PIXEL_PER_SECOND * CurrentZoom;
                cmdCursor.style.left = cmd.GameTime * PIXEL_PER_SECOND * CurrentHorizontalZoom;
            }
        }

        internal void RefreshState()
        {
            BRecorderRuntime.RECORDER_STATE recorderState = BRecorderRuntime.RecordingState;

            switch (recorderState)
            {
                case BRecorderRuntime.RECORDER_STATE.PLAYING:
                    {
                        PauseBtn.SetEnabled(true);
                        StopBtn.SetEnabled(true);

                        PlayBtn.SetEnabled(false);
                        RecordBtn.SetEnabled(false);
                        break;
                    }
                case BRecorderRuntime.RECORDER_STATE.PAUSED:
                    {
                        PlayBtn.SetEnabled(true);
                        StopBtn.SetEnabled(true);

                        PauseBtn.SetEnabled(false);
                        RecordBtn.SetEnabled(false);

                        break;
                    }
                case BRecorderRuntime.RECORDER_STATE.RECORDING:
                    {
                        StopBtn.SetEnabled(true);

                        PlayBtn.SetEnabled(false);
                        PauseBtn.SetEnabled(false);
                        RecordBtn.SetEnabled(false);
                        break;
                    }
                case BRecorderRuntime.RECORDER_STATE.IDLE:
                    {
                        PlayBtn.SetEnabled(true);
                        RecordBtn.SetEnabled(true);

                        PauseBtn.SetEnabled(false);
                        StopBtn.SetEnabled(false);
                        break;
                    }
            }
        }

        private void HandlePauseClicked(ClickEvent evt)
        {
            BRecorderRuntime.PauseSession();
        }

        private void HandleStopClicked(ClickEvent evt)
        {
            BRecorderRuntime.StopRecording();
        }

        private void HandleRecordClicked(ClickEvent evt)
        {
            BRecorderRuntime.StartRecording();
        }

        private void HandlePlayClicked(ClickEvent evt)
        {
            BRecorderRuntime.PlaySession();
        }

        private void HandleMouseLeave(MouseLeaveEvent evt)
        {
            EventSystem.Trigger(new OnTimelineMouseLeave(this, evt));
        }

        private void HandleEditorMouseMove(MouseMoveEvent evt)
        {
            Vector2 localPos = evt.localMousePosition;

            worldMousePostion = Root.LocalToWorld(localPos);
        }

        private void HandleScrollWheel(WheelEvent evt)
        {
            EventSystem.Trigger(new OnTimelineScrollWheel(this, evt));
        }

        private void HandleEditorKeydown(KeyDownEvent evt)
        {
            EventSystem.Trigger(new OnTimelineKeydown(this, evt));
        }

        private void HandleMouseUp(MouseUpEvent evt)
        {
            EventSystem.Trigger(new OnTimelineMouseUp(this, evt));
        }

        private void HandleMouseMove(MouseMoveEvent evt)
        {
            EventSystem.Trigger(new OnTimelineMouseMove(this, evt));
        }

        private void HandleMouseDown(MouseDownEvent evt)
        {
            EventSystem.Trigger(new OnTimelineMouseDown(this, evt));
        }

        private void Destroy()
        {
            // ui
            BRecorderRuntime.OnStateChanged -= HandleStateChanged;
            BRecorderRuntime.OnSessionChanged -= HandleSessionChanged;

            Root.UnregisterCallback<GeometryChangedEvent>(HandleWindowSizeChanged);
            Root.UnregisterCallback<KeyDownEvent>(HandleEditorKeydown);
            PlayBtn.UnregisterCallback<ClickEvent>(HandlePlayClicked);
            RecordBtn.UnregisterCallback<ClickEvent>(HandleRecordClicked);
            StopBtn.UnregisterCallback<ClickEvent>(HandleStopClicked);
            PauseBtn.UnregisterCallback<ClickEvent>(HandlePauseClicked);

            Root.UnregisterCallback<MouseMoveEvent>(HandleEditorMouseMove);
            Timeline.UnregisterCallback<WheelEvent>(HandleScrollWheel);
            Timeline.UnregisterCallback<MouseDownEvent>(HandleMouseDown);
            Timeline.UnregisterCallback<MouseMoveEvent>(HandleMouseMove);
            Timeline.UnregisterCallback<MouseUpEvent>(HandleMouseUp);
            Timeline.UnregisterCallback<MouseLeaveEvent>(HandleMouseLeave);

            // events
            EventSystem.UnlistenAll();

            // actions
            foreach (BRecorderActionBase action in Actions)
            {
                action.Destroy();
            }

            Actions.Clear();
        }

        private void OnDestroy()
        {
            Destroy();
        }

    }
}