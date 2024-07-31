using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using System;

namespace Bloodthirst.Editor.CustomComponent
{
    /// <summary>
    /// <para>A Custom UI Element that renders a mesh onto a texture and displays it as a UI Element</para>
    /// <para>The rendering can be controlled by passing a list of materials along with the camera's position and rotation</para>
    /// </summary>
    public class UIInputListener
    {
        public enum KeyState
        {
            Unpressed = 0b_0000, 
            Pressed = 0b_0001,
            Keyup = 0b_0100, 
            Keydown = 0b_1000
        }

        public struct InputState
        {
            public bool IsMousePressed;
            public KeyState[] KeyboardInput;
            public Vector2 MouseDelta;
            public float ScrollDelta;
        }

        private InputState _inputState;

        public InputState State => _inputState;
        private double _lastTimestamp;
        private readonly VisualElement _sourceElement;

        public UIInputListener(VisualElement sourceElement)
        {
            this._sourceElement = sourceElement;
            sourceElement.focusable = true;
            sourceElement.Focus();

            
            _inputState.KeyboardInput = new KeyState[Enum.GetValues(typeof(KeyCode)).Length];
            _lastTimestamp = EditorApplication.timeSinceStartup;
        }

        public void Unbind()
        {
            _sourceElement.UnregisterCallback<MouseMoveEvent>(HandleMove);
            _sourceElement.UnregisterCallback<MouseDownEvent>(HandleDown);
            _sourceElement.UnregisterCallback<MouseUpEvent>(HandleUp);
            _sourceElement.UnregisterCallback<MouseLeaveEvent>(HandleLeave);

            _sourceElement.UnregisterCallback<KeyDownEvent>(HandleKeydown);
            _sourceElement.UnregisterCallback<KeyUpEvent>(HandleKeyup);
            _sourceElement.UnregisterCallback<WheelEvent>(HandleScroll);
        }


        public void Bind()
        {
            _sourceElement.RegisterCallback<MouseMoveEvent>(HandleMove);
            _sourceElement.RegisterCallback<MouseDownEvent>(HandleDown);
            _sourceElement.RegisterCallback<MouseUpEvent>(HandleUp);
            _sourceElement.RegisterCallback<MouseLeaveEvent>(HandleLeave);

            _sourceElement.RegisterCallback<KeyDownEvent>(HandleKeydown);
            _sourceElement.RegisterCallback<KeyUpEvent>(HandleKeyup);
            _sourceElement.RegisterCallback<WheelEvent>(HandleScroll);
        }
        private void HandleKeyup(KeyUpEvent evt)
        {
            _inputState.KeyboardInput[(int)evt.keyCode] = KeyState.Unpressed;
        }

        private void HandleKeydown(KeyDownEvent evt)
        {
            _inputState.KeyboardInput[(int)evt.keyCode] = KeyState.Pressed;
        }

        private void HandleDown(MouseDownEvent evt)
        {
            _inputState.IsMousePressed = true;
        }

        private void HandleUp(MouseUpEvent evt)
        {
            _inputState.IsMousePressed = false;
        }

        private void HandleLeave(MouseLeaveEvent evt)
        {
            _inputState.IsMousePressed = false;
        }

        private void HandleScroll(WheelEvent evt)
        {
            _inputState.ScrollDelta = -evt.delta.y;
        }


        private void HandleMove(MouseMoveEvent evt)
        {
            _inputState.MouseDelta = evt.mouseDelta;
        }

        public void PostTick()
        {
            _inputState.MouseDelta = Vector2.zero;
            _inputState.ScrollDelta = 0;
        }
    }
}