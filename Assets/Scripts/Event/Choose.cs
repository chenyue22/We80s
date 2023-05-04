using System;
using We80s.GameActor;
using We80s.GameData.Table;

namespace We80s.GameEvent
{
    [Serializable]
    public struct ChoiceData
    {
        public string content;
        public PlayerAttributes reward;
        public int money;
    }
    
    [Serializable]
    public struct ChooseData : IEventModuleData
    {
        public EventModuleType EventModuleType => EventModuleType.MakeAChoice;
        public string dialog;
        public ChoiceData[] choiceDatas;

        public void LoadColumn(ITableColumn column)
        {
            
        }
    }
    
    public class Choose : EventModule<ChooseData>
    {
        protected override void OnSetupData(ChooseData data)
        {
            
        }

        public override void Start()
        {
            
        }

        public override void Stop()
        {
            
        }
    }   
}