using TechC.MagichesBand.Enemy;
using TechC.MagichesBand.Game;
using TechC.MagichesBand.Item;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TechC.MagichesBand.Battle
{
    public class BattlePlayerController : MonoBehaviour,ICharacter
    {
        [SerializeField] private CharacterStatus playerStatus;
        [SerializeField] private GameObject attackCommand;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private ButtonNavigator buttonNavigator;
        [SerializeField] private InventoryUI inventoryUI;
        [SerializeField] private BattleManager battleManager;
        [SerializeField] private ItemSelect itemSelect;
     
        private const float CriticalRate = 0.25f; //クリティカルの確率（今は25％）
        private const int CriticalMultiplier = 2; // クリティカル倍率
        private const float EvasionRate = 0.1f; //回避の確率（今は10％）
    
        private int consumptionMp; //消費Mp

        public int atkDoublingValue; //攻撃上昇補正
        public int defDoublingValue; //防御上昇補正
    
        private void Update()
        {
            if (!buttonNavigator.isInventory || !inventoryUI.isItem) return;
        
            inventoryUI.isItem = false;
            itemSelect.contentParent.gameObject.SetActive(false);
            buttonNavigator.SetInventoryState(false);
            NextState();
        }   
    
        public void Act()
        {
            BattleManager.Instance.EnableActButton();
        }
    
        public void Explosion()
        {
            consumptionMp = 4;
            if ((playerStatus.mp - consumptionMp) >= 0)
            {
                Debug.Log("Explosion");
                messageText.gameObject.SetActive(false);
            
                attackCommand.SetActive(false);
        
                // 敵を取得
                ICharacter enemy = BattleManager.Instance.CurrentEnemy;
        
                // ダメージ計算()
                var damage = Mathf.Max(0, playerStatus.mAtk + atkDoublingValue - enemy.Status.mDef);
                damage = CriticalCalculation(damage);

                // 敵にダメージを与える
                enemy.TakeDamage(damage);   
            }
            else
            {
                Debug.Log("失敗");
            }
        
            NextState();
        }

        public void Slash()
        {
            Debug.Log("Slash");
            messageText.gameObject.SetActive(false);
        
            attackCommand.SetActive(false);
        
            ICharacter enemy = BattleManager.Instance.CurrentEnemy;
        
            int damage = Mathf.Max(0,playerStatus.atk + atkDoublingValue - enemy.Status.def);
        
            damage = CriticalCalculation(damage);
        
            // 敵にダメージを与える
            enemy.TakeDamage(damage);
        
            NextState();
        }

        public void AtkUp()
        {
            if (atkDoublingValue == 16)
            {
                attackCommand.SetActive(false);
                NextState();
                return;
            }

            if (atkDoublingValue == 0)
            {
                atkDoublingValue　= (atkDoublingValue + 1) * 2;
            }else
            {
                atkDoublingValue *= 2;   
            }
        
            Debug.Log(atkDoublingValue);
        
            messageText.gameObject.SetActive(false);
        
            attackCommand.SetActive(false);
        
            NextState();
        }

        public void DefUp()
        {
            switch (defDoublingValue)
            {
                case 16:
                    attackCommand.SetActive(false);
                    NextState();
                    return;
                case 0:
                    defDoublingValue　= (defDoublingValue + 1) * 2;
                    break;
                default:
                    defDoublingValue *= 2;
                    break;
            }

            Debug.Log(defDoublingValue);
        
            messageText.gameObject.SetActive(false);
        
            attackCommand.SetActive(false);
        
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
            var enemy = BattleManager.Instance.CurrentEnemy;
        
            var randomEvasion = Random.Range(0.0f, 1.0f);

            if (randomEvasion < EvasionRate)
            {
                Debug.Log($"回避  残HP: {playerStatus.hp}");
            }
            else
            {
                playerStatus.hp -= damage;
                Debug.Log($"プレイヤーは {damage} ダメージを受けた！ 残HP: {playerStatus.hp}");
            }

            if (playerStatus.hp <= 0)
            {
                ResetStatus();
                GameManager.Instance.playerPosition = new Vector3(-13f, 0.6f, 6);
                enemy.ResetStatus();
                SceneManager.LoadScene("Title");
                Debug.Log("プレイヤーが倒れた！");
            }
        }

        public void NextState()
        {
            ICharacter enemy = BattleManager.Instance.CurrentEnemy;
        
            if (TurnManager.Instance.CurrentTurnPhase == TurnManager.TurnPhase.FirstMove)
            {
                TurnManager.Instance.CurrentTurnPhase = TurnManager.TurnPhase.SecondMove;
                if (BattleManager.Instance.bossPhase2)
                {
                    BattleManager.Instance.bossPhase2 = false;
                    return;
                }
                enemy.Act();
            }else if(TurnManager.Instance.CurrentTurnPhase == TurnManager.TurnPhase.SecondMove)
            {
                TurnManager.Instance.CurrentTurnPhase = TurnManager.TurnPhase.FirstMove;
                TurnManager.Instance.ProceedTurn();
            }
        }
    
        public void ResetStatus()
        {
            playerStatus.hp = playerStatus.maxHp;
            playerStatus.mp = playerStatus.maxMp;
        }

        public void ResetBuff()
        {
            atkDoublingValue = 0;
            defDoublingValue = 0;
        }
    
        public CharacterStatus Status => playerStatus;
    }
}
