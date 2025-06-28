using UnityEngine;

namespace TechC.MagichesBand
{
    [CreateAssetMenu(menuName = "Date/Character Status")]
    public class CharacterStatus : ScriptableObject
    {
        public string enemyName;//敵の名前
        public int maxHp;//MaxHp
        public int maxMp;//MaxMp
        public int hp;//現在のMp
        public int mp;//現在のHp
        public int atk;//攻撃力
        public int def;//防御力
        public int mAtk;//魔法力
        public int mDef;//魔法防御力
        public int agi;//行動速度
    }
}
