using System.Collections.Generic;
using UnityEngine;
using We80s.GameData.Table;
using We80s.Managers;

namespace We80s.ResAssist
{
    public class TextureAtlas
    {
        private Texture2D texture;
        private Sprite[] sprites;
        private Dictionary<int, Sprite> idSprites = new Dictionary<int, Sprite>();
        
        public Sprite this[int index] => sprites[index];
        
        public void SetupColumns(SpriteColumn[] spriteColumns)
        {
            var texture = AssetManager.Instance.LoadObject<Texture2D>(spriteColumns[0].textureId);
            sprites = new Sprite[spriteColumns.Length];
            for (int i = 0; i < spriteColumns.Length; ++i)
            {
                var column = spriteColumns[i];
                var sprite = Sprite.Create(texture, spriteColumns[i].rect, new Vector2(0.5f, 0.5f));
                idSprites[column.id] = sprite;
                sprites[i] = sprite;
            }
        }

        public bool TryGetSpriteById(int id, out Sprite sprite)
        {
            return idSprites.TryGetValue(id, out sprite);
        }
    }   
}