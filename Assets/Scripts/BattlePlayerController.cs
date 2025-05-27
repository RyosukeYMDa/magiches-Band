using UnityEngine;


public class BattlePlayerController : MonoBehaviour,IEnemy
{
    [SerializeField] private CharacterStatus characterStatus;
    
    public void Attack()
    {
        Debug.Log($"{characterStatus.enemyName}（プレイヤー）が攻撃！ ATK: {characterStatus.atk}");
    }
    
    public CharacterStatus Status => characterStatus;
}
