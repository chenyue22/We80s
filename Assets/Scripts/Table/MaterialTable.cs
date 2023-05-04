using System.Collections.Generic;

namespace We80s.GameData.Table
{
    public struct MaterialColumn : ITableColumn, IAssetColumn
    {
        public int id;
        public string assetPath;
        public string AssetPath => assetPath;
    }
    
    public struct MaterialTable : ITable
    {
        public MaterialColumn[] columns;
        public Dictionary<int, int> idToIdxLut;

        public ITableColumn GetColumn(int id)
        {
            int idx;
            if (idToIdxLut.TryGetValue(id, out idx))
            {
                return columns[idx];
            }

            return null;
        }
    }
}