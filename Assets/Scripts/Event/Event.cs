using System;
using System.Collections.Generic;
using We80s.Core;
using We80s.GameData.Table;
using We80s.Managers;

namespace We80s.GameEvent
{
    public class Event
    {
        private EventModule[] eventModules;
        private EventModule currentModule;
        private IUpdate updateModule;
        private int currentIndex;
        
        private static Type[] eventModuleTypes;
        private static Type[] eventModuleDataTypes;
        private static Dictionary<EventModuleType, TableType> eventModuleTableTypeLut;

        private bool finish;
        public bool Finish => finish;

        public static void InitEventTypes()
        {
            eventModuleTableTypeLut = new Dictionary<EventModuleType, TableType>();
            eventModuleTableTypeLut[EventModuleType.Dialog] = TableType.Dialog;
            eventModuleTableTypeLut[EventModuleType.MakeAChoice] = TableType.Choose;
            
            eventModuleTypes = new Type[(int) EventModuleType.Count];
            eventModuleTypes[(int) EventModuleType.Dialog] = typeof(Dialog);
            eventModuleTypes[(int) EventModuleType.MakeAChoice] = typeof(Choose);
            
            eventModuleDataTypes = new Type[(int) EventModuleType.Count];
            eventModuleDataTypes[(int) EventModuleType.Dialog] = typeof(DialogData);
            eventModuleDataTypes[(int) EventModuleType.MakeAChoice] = typeof(ChoiceData);
        }
        
        private EventModule CreateEventModule(EventModuleID id)
        {
            var moduleDataType = eventModuleDataTypes[(int) id.eventModuleType];
            var moduleType = eventModuleTypes[(int) id.eventModuleType];

            TableType tableType;
            if (eventModuleTableTypeLut.TryGetValue(id.eventModuleType, out tableType))
            {
                var column = AssetManager.Instance.LoadColumn(tableType, id.id);
                EventModule module = (EventModule) Activator.CreateInstance(moduleType);
                IEventModuleData eventModuleData = (IEventModuleData) Activator.CreateInstance(moduleDataType);
                eventModuleData.LoadColumn(column);
                module.SetupData(eventModuleData);
                return module;
            }

            return null;
        }
        
        public Event(EventData eventData)
        {
            eventModules = new EventModule[eventData.ids.Length];
            for (int i = 0; i < eventModules.Length; ++i)
            {
                eventModules[i] = CreateEventModule(eventData.ids[i]);
            }
        }

        public bool CanStart()
        {
            if (eventModules == null || eventModules.Length == 0) return false;
            return true;
        }

        public void Start()
        {
            finish = false;
            currentIndex = 0;
            StartModule(currentIndex);
        }

        private bool StartModule(int index)
        {
            if (index < eventModules.Length)
            {
                currentModule?.Stop();
                currentModule = eventModules[index];
                currentModule.Start();
                updateModule = currentModule as IUpdate;
                return true;
            }

            return false;
        }

        public bool CanStop()
        {
            return true;
        }

        public void Stop()
        {
            finish = true;
            currentModule?.Stop();
        }

        public void Update()
        {
            if (finish || currentModule == null) return;
            
            updateModule?.OnUpdate();
            if (currentModule.Finish)
            {
                currentModule.onFinish?.Invoke();
                if (!StartModule(++currentIndex))
                {
                    finish = true;
                }
            }
        }

        public void Release()
        {
            
        }
    }
}