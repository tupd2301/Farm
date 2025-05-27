namespace AthenaFramework.InGameSystem.GameEventSystem
{
    public class GameEvent<T>
    {
        public event System.Action<T> OnEvent;

        public void Invoke(T data) => OnEvent?.Invoke(data);
        public void Subscribe(System.Action<T> listener) => OnEvent += listener;
        public void Unsubscribe(System.Action<T> listener) => OnEvent -= listener;
    }

    public class GameEvent
    {
        public event System.Action OnEvent;

        public void Invoke() => OnEvent?.Invoke();
        public void Subscribe(System.Action listener) => OnEvent += listener;
        public void Unsubscribe(System.Action listener) => OnEvent -= listener;
    }
}
