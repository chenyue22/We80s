using System.Collections.Generic;
using We80s.GameActor;
using We80s.GameEvent;

namespace We80s.GameData.Table
{
    public struct TextureColumn : ITableColumn, IAssetColumn
    {
        public int id;
        public string assetPath;
        public string AssetPath => assetPath;
    }
    
    public struct TextureTable : ITable
    {
        public TextureColumn[] textureColumns;
        public Dictionary<int, int> idToIdxLut;

        public ITableColumn GetColumn(int id)
        {
            int idx;
            if (idToIdxLut.TryGetValue(id, out idx))
            {
                return textureColumns[idx];
            }

            return null;
        }
    }
}