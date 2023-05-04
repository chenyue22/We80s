using UnityEngine;
using We80s.Core;
using We80s.GameEvent;
using We80s.Managers;

namespace We80s.GameActor
{
    public class ActorEvent : IInteractive
    {
        public BoxCollider BoxCollider { get; set; }
        public int actorID;
        private EventBag eventBag;

        public ActorEvent()
        {
            eventBag = new EventBag(16);
        }

        public EventData GetEventData()
        {
            if (!eventBag.Empty) return eventBag[0];
            return default;
        }
    }
}