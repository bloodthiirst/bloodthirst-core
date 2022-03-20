using Bloodthirst.Core.Utils;
using Bloodthirst.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BRecorder
{
    public class BRecorder : EditorWindow
    {
        private const string PATH_UXML = "Packages/com.bloodthirst.bloodthirst-core/Editor/BRecorder/UI/BRecorder.uxml";
        private const string PATH_USS = "Packages/com.bloodthirst.bloodthirst-core/Editor/BRecorder/UI/BRecorder.uss";


        [MenuItem("Bloodthirst Tools/BRecorder")]
        public static void ShowExample()
        {
            BRecorder wnd = GetWindow<BRecorder>();
            wnd.titleContent = new GUIContent("BRecorder");
        }

        internal VisualElement Root { get; set; }
        internal VisualElement RecordBtn => Root.Q<VisualElement>(nameof(RecordBtn));
        internal VisualElement Timeline => Root.Q<VisualElement>(nameof(Timeline));
        internal VisualElement TimelineContainer => Root.Q<VisualElement>(nameof(TimelineContainer));
        internal Scroller TimelineHorizontalScoll => Root.Q<Scroller>(nameof(TimelineHorizontalScoll));

        private Vector2 zoomMinMax;

        private Vector2 horizontalScrollMinMax;
        private float horizontalScrollValue;
        internal float HorizontalScrollValue
        {
            get => horizontalScrollValue;
            set
            {
                horizontalScrollValue = Mathf.Clamp(value, horizontalScrollMinMax.x, horizontalScrollMinMax.y);
                ScrollChanged();
            }
        }

        internal Vector2 worldMousePostion;

        private float currentZoom;
        internal float CurrentZoom
        {
            get => currentZoom;
            set
            {
                float old = currentZoom;
                currentZoom = Mathf.Clamp(value, zoomMinMax.x, zoomMinMax.y);
                ViewPortChanged(old);
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
            /*
            Debug.Log($"Window pos : { position}");
            Debug.Log($"Mouse pos : { worldMousePostion}");
            */
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
            zoomMinMax = new Vector2(1, 10);
            CurrentZoom = 1;
        }

        private void ListenUI()
        {
            Root.RegisterCallback<MouseMoveEvent>(HandleEditorMouseMove);
            Timeline.RegisterCallback<WheelEvent>(HandleScrollWheel);
            Timeline.RegisterCallback<MouseDownEvent>(HandleMouseDown);
            Timeline.RegisterCallback<MouseMoveEvent>(HandleMouseMove);
            Timeline.RegisterCallback<MouseUpEvent>(HandleMouseUp);
            Timeline.RegisterCallback<MouseLeaveEvent>(HandleMouseLeave);
            TimelineHorizontalScoll.RegisterCallback<ChangeEvent<float>>(HandleScrollChanged);
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

        private void HandleScrollChanged(ChangeEvent<float> evt)
        {
            float t = Mathf.InverseLerp(horizontalScrollMinMax.x, horizontalScrollMinMax.y, evt.newValue);
            float invert_t = 1 - t;
            float correct_value = Mathf.Lerp(horizontalScrollMinMax.x, horizontalScrollMinMax.y, invert_t);

            EventSystem.Trigger(new OnTimelineHorizontalScrollChanged(this, correct_value));
        }


        private void ScrollChanged()
        {
            float t = Mathf.InverseLerp(horizontalScrollMinMax.x, horizontalScrollMinMax.y, horizontalScrollValue);
            float slider_t = 1 - t;
            float slider_value = Mathf.Lerp(horizontalScrollMinMax.x, horizontalScrollMinMax.y, slider_t);

            TimelineHorizontalScoll.slider.SetValueWithoutNotify(slider_value);


            // scroll offset
            Vector3 pos = Timeline.transform.position;
            pos.x = -horizontalScrollValue;


            Timeline.transform.position = pos;

        }

        private void ViewPortChanged(float oldZoom)
        {
            // zoom
            Timeline.transform.scale = new Vector3(CurrentZoom, CurrentZoom, CurrentZoom);

            // recalculate scroll bounds
            float timelineWidth = Timeline.worldBound.width;

            float containerWidth = TimelineContainer.worldBound.width;

            float differenceInPx = timelineWidth - containerWidth;

            bool hasScroll = differenceInPx > 0;

            TimelineContainer.style.height = Timeline.worldBound.height;

            TimelineHorizontalScoll.Display(hasScroll);

            if (!hasScroll)
            {
                horizontalScrollMinMax.x = 0;
                horizontalScrollMinMax.y = 0;


                TimelineHorizontalScoll.lowValue = horizontalScrollMinMax.x;
                TimelineHorizontalScoll.highValue = horizontalScrollMinMax.y;

                TimelineHorizontalScoll.value = 0;
                horizontalScrollValue = TimelineHorizontalScoll.value;

                Timeline.transform.position = Vector3.zero;
                return;
            }



            // save old h-scroll
            Vector2 oldMinMax = horizontalScrollMinMax;
            float oldHorizontal = horizontalScrollValue;
            float t = Mathf.InverseLerp(oldMinMax.x, oldMinMax.y, oldHorizontal);

            // remap old to new
            Vector2 newMinMax = new Vector2();
            newMinMax.x = 0;
            newMinMax.y = differenceInPx;
            float newHorizontal = Mathf.Lerp(newMinMax.x, newMinMax.y, t);

            // assign
            horizontalScrollMinMax = newMinMax;

            TimelineHorizontalScoll.lowValue = horizontalScrollMinMax.x;
            TimelineHorizontalScoll.highValue = horizontalScrollMinMax.y;

            if (oldZoom == 1)
            {
                horizontalScrollValue = 0;
            }
            else
            {
                horizontalScrollValue = newHorizontal;
            }

            ScrollChanged();
        }

        private void Destroy()
        {
            // ui
            Root.UnregisterCallback<MouseMoveEvent>(HandleEditorMouseMove);
            Timeline.UnregisterCallback<WheelEvent>(HandleScrollWheel);
            Timeline.UnregisterCallback<MouseDownEvent>(HandleMouseDown);
            Timeline.UnregisterCallback<MouseMoveEvent>(HandleMouseMove);
            Timeline.UnregisterCallback<MouseUpEvent>(HandleMouseUp);
            Timeline.UnregisterCallback<MouseLeaveEvent>(HandleMouseLeave);
            TimelineHorizontalScoll.UnregisterCallback<ChangeEvent<float>>(HandleScrollChanged);

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