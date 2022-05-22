namespace Testwork.Events
{
    public interface IGameEventListener {}
    
    public interface IGameEventListener<in T> : IGameEventListener where T : IGameEvent
    {
        void OnEvent(T gameEvent);
    }
}