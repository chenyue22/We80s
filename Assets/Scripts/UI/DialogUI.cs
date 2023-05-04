using UnityEngine;
using UnityEngine.UI;
using We80s.Managers;

namespace We80s.UI
{
    public class DialogUI : UIBase
    {
        public Text text;
        public Image leftCharacterPainting;
        public Image rightCharacterPainting;
        private int leftCharacterPaintingId;
        private int rightCharacterPaintingId;

        public override void Release()
        {
            
        }

        public void SetCharacterPaintingId(int id, bool isLeftCharacter)
        {
            if (isLeftCharacter)
            {
                if (leftCharacterPaintingId != id)
                {
                    if (leftCharacterPainting.sprite)
                    {
                        AssetManager.Instance.Release(leftCharacterPainting.sprite);
                    }
                    
                }
            }
        }
    }   
}