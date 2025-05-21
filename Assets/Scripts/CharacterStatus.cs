using UnityEngine;

[CreateAssetMenu(menuName = "Date/Character Status")]
public class CharacterStatus : ScriptableObject
{
    public string enemyName;//敵の名前
    public int maxHp;//MaxHp
    public int maxMp;//MaxMp
    public int atk;//攻撃力
    public int def;//防御力
    public int mp;//魔法力
    public int res;//魔法抵抗力
    public int agi;//行動速度
}
