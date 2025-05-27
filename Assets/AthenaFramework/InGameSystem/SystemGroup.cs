using System;
using System.Collections.Generic;
using UnityEngine;

namespace AthenaFramework.InGameSystem
{
    public class SystemGroup
    {
        private Dictionary<Type, GameSystem> _systems;

        public SystemGroup()
        {
            _systems = new();
        }

        public void RegisterSystem<T>(T system) where T : GameSystem
        {
            var type = typeof(T);
            if (_systems.ContainsKey(type))
            {
                Debug.LogError($"System already registered: {type}");
            }
            else
            {
                _systems.Add(type, system);
                system.Init();
            }
        }

        public void UnregisterSystem<T>() where T : GameSystem
        {
            var type = typeof(T);
            if (_systems.ContainsKey(type))
            {
                _systems[type].Destroy();
                _systems.Remove(type);
            }
        }

        public T GetSystem<T>() where T : GameSystem
        {
            return _systems[typeof(T)] as T;
        }
    }
}
