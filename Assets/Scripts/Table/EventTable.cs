using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using We80s.GameEvent;

namespace We80s.GameData.Table
{

    public struct EventColumn : ITableColumn
    {
        public int id;
        public EventModuleID[] eventModuleIds;
    }
    
    public struct EventTable : ITable
    {
        public EventColumn[] eventColumns;
        public Dictionary<int, int> idToIdxLut;

        public ITableColumn GetColumn(int id)
        {
            int idx;
            if (idToIdxLut.TryGetValue(id, out idx))
            {
                return eventColumns[idx];
            }

            return null;
        }
    }   
}