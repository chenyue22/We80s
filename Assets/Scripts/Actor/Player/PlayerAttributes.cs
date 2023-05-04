using System;

namespace We80s.GameActor
{
    public enum PlayerAttributeType
    {
        IQ,
        EQ,
        Memory,
        Vitality,
        Health,
        Patience,
        LimbSynergy,
        Appearance,
        Face,
        Happiness,
        Voice
    }
    
    [Serializable]
    public struct PlayerAttributes
    {
        public int iq;                  //智商
        public int eq;                  //情商
        public int memory;              //记忆力
        public int vitality;            //体力
        public int health;              //健康
        public int patience;            //耐心
        public int limbSynergy;         //肢体协同
        public int appearance;          //长相
        public int face;                //面子
        public int happiness;           //幸福
        public int voice;               //表达能力

        public void AddAttribute(PlayerAttributeType type, int value)
        {
            switch (type)
            {
                case PlayerAttributeType.IQ: iq += value;
                    break;
                case PlayerAttributeType.EQ: eq += value;
                    break;
                case PlayerAttributeType.Memory: memory += value;
                    break;
                case PlayerAttributeType.Vitality: vitality += value;
                    break;
                case PlayerAttributeType.Health: health += value;
                    break;
                case PlayerAttributeType.Patience: patience += value;
                    break;
                case PlayerAttributeType.LimbSynergy: limbSynergy += value;
                    break;
                case PlayerAttributeType.Appearance: appearance += value;
                    break;
                case PlayerAttributeType.Face: face += value;
                    break;
                case PlayerAttributeType.Happiness: happiness += value;
                    break;
                case PlayerAttributeType.Voice: voice += value;
                    break;
            }
        }

        public static PlayerAttributes operator+(PlayerAttributes a1, PlayerAttributes a2)
        {
            return new PlayerAttributes
            {
                iq = a1.iq + a2.iq,
                eq = a1.eq + a2.eq,
                memory = a1.memory + a2.memory,
                vitality = a1.vitality + a2.vitality,
                health = a1.health + a2.health,
                patience = a1.patience + a2.patience,
                limbSynergy = a1.limbSynergy + a2.limbSynergy,
                appearance = a1.appearance + a2.appearance,
                face = a1.face + a2.face,
                happiness = a1.happiness + a2.happiness,
                voice = a1.voice + a2.voice
            };
        }
    }
}
