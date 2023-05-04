using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace We80s.GameEvent
{
    public abstract class EventModule
    {
        protected bool finish;
        public bool Finish => finish;
        
        public Action onFinish;

        public virtual void SetupData(IEventModuleData eventData)
        {
            
        }
        public abstract void Start();
        public abstract void Stop();
    }

    public abstract class EventModule<T> : EventModule where T : IEventModuleData
    {
        public sealed override void SetupData(IEventModuleData eventData)
        {
            OnSetupData((T) eventData);
        }

        protected virtual void OnSetupData(T data)
        {
            
        }
    }
}
