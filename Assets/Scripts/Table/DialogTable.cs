using System.Collections.Generic;
using We80s.GameEvent;

namespace We80s.GameData.Table
{
    public struct DialogColumn : ITableColumn
    {
        public int id;
        public Speach[] speaches;
    }
    
    public struct DialogTable : ITable
    {
        public DialogColumn[] dialogColumns;
        public Dictionary<int, int> idToIdxLut;

        public ITableColumn GetColumn(int id)
        {
            int idx;
            if (idToIdxLut.TryGetValue(id, out idx))
            {
                return dialogColumns[idx];
            }

            return null;
        }
    }   
}