using System;
using We80s.Core;
using We80s.GameData.Table;
using We80s.Managers;
using We80s.UI;

namespace We80s.GameEvent
{
    public struct Speach
    {
        public int speakerId;
        public string content;

        public Speach(int id, string c)
        {
            speakerId = id;
            content = c;
        }
    }
    
    [Serializable]
    public struct DialogData : IEventModuleData
    {
        public Speach[] speaches;
        public EventModuleType EventModuleType => EventModuleType.Dialog;

        public void LoadColumn(ITableColumn column)
        {
            DialogColumn dialogColumn = (DialogColumn) column;
            speaches = dialogColumn.speaches;
        }
    }
    
    public class Dialog : EventModule<DialogData>, IUpdate
    {
        private Speach[] speaches;
        private int index;
        private DialogUI dialogUi;
        
        protected override void OnSetupData(DialogData dialogData)
        {
            speaches = dialogData.speaches;
        }

        public override void Start()
        {
            index = 0;
            dialogUi = (DialogUI) UIManager.Instance.Open(SingletonUI.Dialog);
        }

        public override void Stop()
        {
            
        }

        public void OnUpdate()
        {
            
        }
    }   
}