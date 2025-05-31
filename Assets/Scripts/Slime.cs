using UnityEngine;

public class Slime : MonoBehaviour,IEnemy
{
    [SerializeField] private CharacterStatus characterStatus;
    [SerializeField] private BattlePlayerController battlePlayerController;
    
    private const float CriticalRate = 0.25f; //クリティカルの確率（今は25％）
    private const int CriticalMultiplier = 2; // クリティカル倍率
    private const float EvasionRate = 0.1f; //回避の確率（今は10％）
    public CharacterStatus Status => characterStatus;
    
    public void Act()
    {
        var damage = 0;
        
        var randomAttack = Random.Range(0, 1);

        switch (randomAttack)
        {
            case 0:
                Debug.Log("Charge");
                // プレイヤーへのダメージ計算
                damage = Mathf.Max(0, characterStatus.atk - battlePlayerController.Status.def);
                damage = CriticalCalculation(damage);
                battlePlayerController.TakeDamage(damage);
                break;
            case 1:
                Debug.Log("Fire");
                // プレイヤーへのダメージ計算
                damage = Mathf.Max(0, characterStatus.mAtk - battlePlayerController.Status.mDef);
                damage = CriticalCalculation(damage);
                battlePlayerController.TakeDamage(damage);
                break;
        }
        NextState();
    }
    
    /// <summary>
    /// criticalの処理（クリティカルの確率を個別に変えたいのでまとめておかない）
    /// </summary>
    /// <param name="damage"></param>
    private int CriticalCalculation(int damage)
    {
        // ランダム値を生成
        float randomCritical = Random.Range(0.0f, 1.0f);
        
        //ランダム値よりクリティカル確率が上だったら、クリティカルがでる
        if (randomCritical < CriticalRate)
        {
            damage *= CriticalMultiplier;
            Debug.Log("Critical");
        }
        return damage;
    }
    
    public void TakeDamage(int damage)
    {
        var randomEvasion = Random.Range(0.0f, 1.0f);

        if (randomEvasion < EvasionRate)
        {
            Debug.Log($"回避  残HP: {characterStatus.maxHp}");
        }
        else
        {
            characterStatus.maxHp -= damage;
            Debug.Log($"{gameObject.name} は {damage} ダメージを受けた！ 残HP: {characterStatus.maxHp}");   
        }

        if (characterStatus.maxHp <= 0)
        {
            Debug.Log($"{gameObject.name} を撃破！");
            Destroy(gameObject);
        }
    }

    public void NextState()
    {
        Debug.Log("NextState");
        if (TurnManager.Instance.CurrentTurnPhase == TurnManager.TurnPhase.FirstMove)
        {
            TurnManager.Instance.CurrentTurnPhase = TurnManager.TurnPhase.SecondMove;
            battlePlayerController.Act();
        }else if (TurnManager.Instance.CurrentTurnPhase == TurnManager.TurnPhase.SecondMove)
        {
            TurnManager.Instance.CurrentTurnPhase = TurnManager.TurnPhase.FirstMove;
            TurnManager.Instance.ProceedTurn();
        }
    }
}
