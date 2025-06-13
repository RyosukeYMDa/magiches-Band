using UnityEngine;
using UnityEngine.SceneManagement;

public class BossPhase2 : MonoBehaviour,IEnemy
{
    [SerializeField] private CharacterStatus bossPhase2Status;
    [SerializeField] private BattlePlayerController battlePlayerController;
    
    private const float CriticalRate = 0.25f; //クリティカルの確率（今は25％）
    private const int CriticalMultiplier = 2; // クリティカル倍率
    private const float EvasionRate = 0.1f; //回避の確率（今は10％）
    
    private int consumptionMp; //消費Mp
    public CharacterStatus Status => bossPhase2Status;
    
    public void Act()
    {
        var damage = 0;
        
        var randomAttack = Random.Range(0, 2);

        switch (randomAttack)
        {
            case 0:
                Debug.Log("Charge");
                // プレイヤーへのダメージ計算
                damage = Mathf.Max(0, bossPhase2Status.atk - battlePlayerController.Status.def);
                damage = CriticalCalculation(damage);
                battlePlayerController.TakeDamage(damage);
                break;
            case 1:
                consumptionMp = 1;
                if ((bossPhase2Status.mp - consumptionMp) >= 0)
                {
                    Debug.Log("Fire");
                    // プレイヤーへのダメージ計算
                    damage = Mathf.Max(0, bossPhase2Status.mAtk - battlePlayerController.Status.mDef);
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
            Debug.Log($"回避  残HP: {bossPhase2Status.hp}");
        }
        else
        {
            bossPhase2Status.hp -= damage;
            Debug.Log($"{gameObject.name} は {damage} ダメージを受けた！ 残HP: {bossPhase2Status.hp}");   
        }

        if (bossPhase2Status.hp <= 0)
        {
            Destroy(gameObject);
            Debug.Log($"{gameObject.name} を撃破！");
            ResetStatus();
            BattleManager.Instance.SavePlayerInventory();
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
            Debug.Log("Act");
        }else if (TurnManager.Instance.CurrentTurnPhase == TurnManager.TurnPhase.SecondMove)
        {
            TurnManager.Instance.CurrentTurnPhase = TurnManager.TurnPhase.FirstMove;
            TurnManager.Instance.ProceedTurn();
            Debug.Log("ProceedTurn");
        }
    }

    public void ResetStatus()
    {
        bossPhase2Status.hp = bossPhase2Status.maxHp;
        bossPhase2Status.mp = bossPhase2Status.maxMp;
    }
}