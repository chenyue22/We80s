using System.Collections.Generic;
using We80s.GameActor;
using We80s.GameEvent;

namespace We80s.GameData.Table
{
    public struct Choice
    {
        public string content;
        public PlayerAttributes reward;
        public int money;
        public int evtId;
        public EventModuleType eventModuleType;
        public int moduleId;
    }
    
    public struct ChooseColumn : ITableColumn
    {
        public int id;
        public Choice[] choices;
    }
    
    public struct ChooseTable : ITable
    {
        public ChooseColumn[] chooseColumns;
        public Dictionary<int, int> idToIdxLut;

        public ITableColumn GetColumn(int id)
        {
            int idx;
            if (idToIdxLut.TryGetValue(id, out idx))
            {
                return chooseColumns[idx];
            }

            return null;
        }
    }   
}