using System;
using UnityEngine;

namespace We80s.GameActor
{
    public class Actor : MonoBehaviour
    {
        public ActorEvent actorEvent = new ActorEvent();
        public ActorController Controller { get; set; }

        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            Controller = CreateController();
            Controller.interactive = actorEvent;
            TryGetComponent(out Controller.agent);
            BoxCollider boxCollider;
            if (TryGetComponent(out boxCollider))
            {
                actorEvent.BoxCollider = boxCollider;
            }
        }

        protected virtual ActorController CreateController()
        {
            return new ActorController();
        }
    }
}
