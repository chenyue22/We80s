using System.Collections.Generic;
using UnityEngine;

namespace We80s.GameData.Table
{
    public struct SpriteColumn : ITableColumn
    {
        public int id;
        public int textureId;
        public Rect rect;
    }
    
    public struct SpriteTable : ITable
    {
        public SpriteColumn[] columns;
        public Dictionary<int, int> idToIdxLut;
        public Dictionary<int, Vector2Int> spriteRangeLut;

        public ITableColumn GetColumn(int id)
        {
            int idx;
            if (idToIdxLut.TryGetValue(id, out idx))
            {
                return columns[idx];
            }

            return null;
        }

        public SpriteColumn[] GetTextureColumns(int textureId)
        {
            SpriteColumn[] columns = null;
            Vector2Int range;
            if (spriteRangeLut.TryGetValue(textureId, out range))
            {
                columns = new SpriteColumn[range.y];
                for (int i = range.x; i < columns.Length; ++i)
                {
                    columns[i] = this.columns[i];
                }
            }

            return columns;
        }
    }
}