using System;
using We80s.Core;
using We80s.GameActor;

namespace We80s.GameEvent
{
    [Serializable]
    public struct EventModuleID
    {
        public EventModuleType eventModuleType;
        public int id;
    }
    
    public struct EventData : IBagElement
    {
        public bool Empty { get; set; }
        public int eventID;
        public EventModuleID[] ids;
        public PlayerAttributes requirement;
    }
}