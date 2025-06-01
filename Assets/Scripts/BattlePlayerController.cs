using UnityEngine;
using UnityEngine.SceneManagement;


public class BattlePlayerController : MonoBehaviour,IEnemy
{
    [SerializeField] private CharacterStatus characterStatus;
    [SerializeField] private GameObject attackCommand;
    
    private const float CriticalRate = 0.25f; //クリティカルの確率（今は25％）
    private const int CriticalMultiplier = 2; // クリティカル倍率
    private const float EvasionRate = 0.1f; //回避の確率（今は10％）
    
    public void Act()
    {
        BattleManager.Instance.EnableActButton();
    }
    
    public void Explosion()
    {
        Debug.Log("Explosion");
     
        attackCommand.SetActive(false);
        
        // 敵を取得
        IEnemy enemy = BattleManager.Instance.CurrentEnemy;
        
        // ダメージ計算()
        var damage = Mathf.Max(0, characterStatus.mAtk - enemy.Status.mDef);
        
        damage = CriticalCalculation(damage);

        // 敵にダメージを与える
        enemy.TakeDamage(damage);
        
        NextState();
    }

    public void Slash()
    {
        Debug.Log("Slash");
        
        attackCommand.SetActive(false);
        
        IEnemy enemy = BattleManager.Instance.CurrentEnemy;
        
        int damage = Mathf.Max(0,characterStatus.atk - enemy.Status.def);
        
        damage = CriticalCalculation(damage);
        
        // 敵にダメージを与える
        enemy.TakeDamage(damage);
        
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
    
    
    /// <summary>
    /// 相手からダメージを受け取り、確率で回避をさせる
    /// </summary>
    /// <param name="damage"></param>
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
            Debug.Log($"プレイヤーは {damage} ダメージを受けた！ 残HP: {characterStatus.maxHp}");
        }

        if (characterStatus.maxHp <= 0)
        {
            characterStatus.maxHp = 26;
            GameManager.Instance.playerPosition = new Vector3(-13f, 0.6f, 6);
            SceneManager.LoadScene("Title");
            Debug.Log("プレイヤーが倒れた！");
        }
    }

    public void NextState()
    {
        IEnemy enemy = BattleManager.Instance.CurrentEnemy;
        
        if (TurnManager.Instance.CurrentTurnPhase == TurnManager.TurnPhase.FirstMove)
        {
            TurnManager.Instance.CurrentTurnPhase = TurnManager.TurnPhase.SecondMove;
            enemy.Act();
        }else if(TurnManager.Instance.CurrentTurnPhase == TurnManager.TurnPhase.SecondMove)
        {
            TurnManager.Instance.CurrentTurnPhase = TurnManager.TurnPhase.FirstMove;
            TurnManager.Instance.ProceedTurn();
        }
    }
    
    public CharacterStatus Status => characterStatus;
}
