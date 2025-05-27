using System.Collections.Generic;

namespace AthenaFramework.InGameSystem.GameEventSystem
{
    public class GameEventSystem : GameSystem
    {
        private readonly Dictionary<string, object> _stringToEvent = new();

        public override void Init() {}
        public override void Destroy() {}

        /// <summary>
        /// Get event by event name.
        /// Using tuple if you want to pass multiple parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public GameEvent<T> GetEvent<T>(string eventName)
        {
            if (!_stringToEvent.TryGetValue(eventName, out var gameEvent))
            {
                gameEvent = new GameEvent<T>();
                _stringToEvent[eventName] = gameEvent;
            }
            return (GameEvent<T>)gameEvent;
        }

        public GameEvent GetEvent(string eventName)
        {
            if (!_stringToEvent.TryGetValue(eventName, out var gameEvent))
            {
                gameEvent = new GameEvent();
                _stringToEvent[eventName] = gameEvent;
            }
            return (GameEvent)gameEvent;
        }
    }
}
