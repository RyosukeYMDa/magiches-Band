using UnityEngine;
using UnityEngine.Serialization;

namespace TechC.MagichesBand.Game
{
    /// <summary>
    /// CharacterのStatusを管理するClass
    /// </summary>
    [CreateAssetMenu(menuName = "Date/Character Status")]
    public class CharacterStatus : ScriptableObject
    {
        [FormerlySerializedAs("enemyName")] public string characterName;//敵の名前
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
