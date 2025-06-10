using UnityEngine;
using UnityEngine.SceneManagement;

public class Slime : MonoBehaviour,IEnemy
{
    [SerializeField] private CharacterStatus characterStatus;
    [SerializeField] private BattlePlayerController battlePlayerController;
    [SerializeField] private BattleManager battleManager;
    
    private const float CriticalRate = 0.25f; //クリティカルの確率（今は25％）
    private const int CriticalMultiplier = 2; // クリティカル倍率
    private const float EvasionRate = 0.1f; //回避の確率（今は10％）
    
    private int consumptionMp; //消費Mp
    public CharacterStatus Status => characterStatus;
    
    public void Act()
    {
        var damage = 0;
        
        var randomAttack = Random.Range(0, 2);

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
                consumptionMp = 1;
                if ((characterStatus.mp - consumptionMp) >= 0)
                {
                    Debug.Log("Fire");
                    // プレイヤーへのダメージ計算
                    damage = Mathf.Max(0, characterStatus.mAtk - battlePlayerController.Status.mDef);
                    damage = CriticalCalculation(damage);
                    battlePlayerController.TakeDamage(damage);   
                }
                else
                {
                    Debug.Log("失敗");
                }
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
            Debug.Log($"回避  残HP: {characterStatus.hp}");
        }
        else
        {
            characterStatus.hp -= damage;
            Debug.Log($"{gameObject.name} は {damage} ダメージを受けた！ 残HP: {characterStatus.hp}");   
        }

        if (characterStatus.hp <= 0)
        {
            Destroy(gameObject);
            Debug.Log($"{gameObject.name} を撃破！");
            characterStatus.hp = characterStatus.maxHp;
            characterStatus.mp = characterStatus.maxMp;
            battleManager.SavePlayerInventory();
            SceneManager.LoadScene("MainScene");
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
