using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Bloodthirst.Core.BInput
{
    /// <summary>
    /// This is a "pseudo code" version of what the service provider should expose as methods
    /// Ofc this is only to get the point across
    /// </summary>
    public static class ServiceProvider
    {
        public static void RegisterSingleton<T>(T instance) { }
        public static void UnregisterSingleton<T>(T instance) { }
        public static T GetSingleton<T>() => default;
    }


    /// <summary>
    /// This component is placed permanantly in the scene (some kind of "GameSetup" scene or "InitGame" scene
    /// make sure it's initalized correctly which is project dependent
    /// this might be overkill but this is how i update everything almost from one behaviour
    /// </summary>
    public class UpdatableBehaviour : MonoBehaviour
    {
        private IBUpdateHandler updateHandler = new UpdateHandler();

        private void OnEnable()
        {
            ServiceProvider.RegisterSingleton(updateHandler);
        }

        private void Update()
        {
            updateHandler.Tick();
        }

        private void OnDisable()
        {
            ServiceProvider.UnregisterSingleton(updateHandler);
        }
    }

    /// <summary>
    /// The updater interface
    /// </summary>
    public interface IBUpdateHandler
    {
        void Add(IBUpdatable updatable);
        void Remove(IBUpdatable updatable);
        void Tick();
    }
 
    /// <summary>
    /// The updater concrete implemention
    /// </summary>
    public class UpdateHandler : IBUpdateHandler
    {
        private HashSet<IBUpdatable> updatables = new HashSet<IBUpdatable>();

        void IBUpdateHandler.Add(IBUpdatable updatable)
        {
            updatables.Add(updatable);
        }

        void IBUpdateHandler.Remove(IBUpdatable updatable)
        {
            updatables.Remove(updatable);
        }

        void IBUpdateHandler.Tick()
        {
            foreach (IBUpdatable u in updatables)
            {
                u.OnTick();
            }
        }
    }


    /// <summary>
    /// Anything that want to update every frame should implement this , in our case it's an input system
    /// </summary>
    public interface IBUpdatable
    {
        void OnTick();
    }

    public enum MouseButton
    {
        Left = 0,
        Middle = 1,
        Right = 2,
    }

    /// <summary>
    /// example of the stuff that some input system might provide
    /// </summary>
    public interface IBInput
    {
        public event Action<KeyCode> OnKeyDown;
        public event Action<KeyCode> OnKeyUp;
        public event Action<MouseButton> OnMouseDown;
        public event Action<MouseButton> OnMouseUp;
    }

    /// <summary>
    /// the actual implementation
    /// </summary>
    public class BInput : IBInput, IBUpdatable
    {
        private static readonly KeyCode[] allKeys = (KeyCode[])Enum.GetValues(typeof(KeyCode));
        private static readonly MouseButton[] allMouseBtns = new MouseButton[] { MouseButton.Left, MouseButton.Middle, MouseButton.Right };

        public event Action<KeyCode> OnKeyDown;
        public event Action<KeyCode> OnKeyUp;
        public event Action<MouseButton> OnMouseDown;
        public event Action<MouseButton> OnMouseUp;

        void IBUpdatable.OnTick()
        {
            // handle keyboard
            for (int i = 0; i < allKeys.Length; i++)
            {
                KeyCode key = allKeys[i];

                if (Input.GetKeyDown(key))
                {
                    OnKeyDown?.Invoke(key);
                }

                if (Input.GetKeyUp(key))
                {
                    OnKeyUp?.Invoke(key);
                }
            }

            // handle mouse
            for (int i = 0; i < allMouseBtns.Length; i++)
            {
                MouseButton btn = allMouseBtns[i];
                int btnAsInt = (int)btn;

                if (Input.GetMouseButtonDown(btnAsInt))
                {
                    OnMouseDown?.Invoke(btn);
                }

                if (Input.GetMouseButtonUp(btnAsInt))
                {
                    OnMouseUp?.Invoke(btn);
                }


            }
        }
    }
}
