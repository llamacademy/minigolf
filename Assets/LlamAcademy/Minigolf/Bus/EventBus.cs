namespace LlamAcademy.Minigolf.Bus
{
    public static class EventBus<T>
    {
        public delegate void Event(T evt);

        public static event Event OnEvent;

        public static void Raise(T evt) => OnEvent?.Invoke(evt);
    }
}
