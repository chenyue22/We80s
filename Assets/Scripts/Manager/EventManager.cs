using We80s.Core;
using We80s.GameData.Table;
using We80s.GameEvent;
using Event = We80s.GameEvent.Event;

namespace We80s.Managers
{
    public class EventManager : LazyInstance<EventManager>, IManager, IUpdate
    {
        public bool Loaded { get; set; }
        private Event currentEvent;

        public void Init()
        {
            Event.InitEventTypes();
            Loaded = true;
        }

        public void Start()
        {

        }

        public void Release()
        {

        }

        public void PlayEvent(int id)
        {
            if (currentEvent != null)
            {
                if (currentEvent.CanStop())
                {
                    return;
                }
                currentEvent.Stop();
                currentEvent.Release();
            }
            var eventColumn = (EventColumn) AssetManager.Instance.LoadColumn(TableType.Event, id);
            EventData eventData = new EventData();
            eventData.eventID = eventColumn.id;
            eventData.ids = eventColumn.eventModuleIds;
            currentEvent = new Event(eventData);
            if (currentEvent.CanStart())
            {
                currentEvent.Start();
            }
        }

        public void StopEvent()
        {
            
        }

        public void OnUpdate()
        {
            if (!currentEvent.Finish)
                currentEvent?.Update();
        }
    }   
}