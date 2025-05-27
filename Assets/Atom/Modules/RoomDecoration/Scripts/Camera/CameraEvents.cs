using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomDecoration.CameraUtilities
{
    public struct CameraZoomEvent
    {
        static private event Delegate OnEvent;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)] private static void RuntimeInitialization() { OnEvent = null; }
        static public void Register(Delegate callback) { OnEvent += callback; }
        static public void Unregister(Delegate callback) { OnEvent -= callback; }

        public delegate void Delegate(Vector3 targetPosition);

        static public void Trigger(Vector3 targetPosition)
        {
            OnEvent?.Invoke(targetPosition);
        }
    }

    public struct CameraZoomInStopEvent
    {
        static private event Delegate OnEvent;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)] private static void RuntimeInitialization() { OnEvent = null; }
        static public void Register(Delegate callback) { OnEvent += callback; }
        static public void Unregister(Delegate callback) { OnEvent -= callback; }

        public delegate void Delegate();

        static public void Trigger()
        {
            OnEvent?.Invoke();
        }
    }

    public struct CameraZoomOutStopEvent
    {
        static private event Delegate OnEvent;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)] private static void RuntimeInitialization() { OnEvent = null; }
        static public void Register(Delegate callback) { OnEvent += callback; }
        static public void Unregister(Delegate callback) { OnEvent -= callback; }

        public delegate void Delegate();

        static public void Trigger()
        {
            OnEvent?.Invoke();
        }
    }
}
