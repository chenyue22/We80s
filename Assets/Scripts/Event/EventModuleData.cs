using We80s.GameData.Table;

namespace We80s.GameEvent
{
    public enum EventModuleType
    {
        Dialog        = 0,
        MakeAChoice,
        MiniGame,
        PlayAStory,
        Count
    }

    public interface IEventModuleData
    {
        EventModuleType EventModuleType { get; }
        void LoadColumn(ITableColumn column);
    }
}
