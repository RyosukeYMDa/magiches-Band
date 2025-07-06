using TechC.MagichesBand.Battle;
using TechC.MagichesBand.Game;
using TechC.MagichesBand.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TechC.MagichesBand.Enemy
{
    public class BossPhase2 : MonoBehaviour,ICharacter
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
            if(StickRotationDetector.Instance.defeatedEnemy) return;
        
            var damage = 0;
        
            var randomAttack = Random.Range(0, 3);

            switch (randomAttack)
            {
                case 0:
                    Debug.Log("Charge");
                    // プレイヤーへのダメージ計算
                    damage = Mathf.Max(0, bossPhase2Status.atk - battlePlayerController.Status.def);
                    damage = CriticalCalculation(damage);
                    battlePlayerController.TakeDamage(damage, ICharacter.AttackType.Physical);
                    break;
                case 1:
                    consumptionMp = 1;
                    if ((bossPhase2Status.mp - consumptionMp) >= 0)
                    {
                        Debug.Log("Fire");
                        // プレイヤーへのダメージ計算
                        damage = Mathf.Max(0, bossPhase2Status.mAtk - battlePlayerController.Status.mDef);
                        damage = CriticalCalculation(damage);
                        battlePlayerController.TakeDamage(damage, ICharacter.AttackType.Magical);   
                    }
                    else
                    {
                        MessageWindow.Instance.DisplayMessage("SkillFailed");
                    }
                    break;
                case 2:
                    consumptionMp = 1;
                    if ((bossPhase2Status.mp - consumptionMp) >= 0)
                    {
                        MessageWindow.Instance.DisplayMessage("NegationBuff");
                        battlePlayerController.ResetBuff();
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
    
        public void TakeDamage(int damage, ICharacter.AttackType type)
        {
            var randomEvasion = Random.Range(0.0f, 1.0f);

            if (randomEvasion < EvasionRate)
            {
                Debug.Log($"回避  残HP: {bossPhase2Status.hp}");
                MessageWindow.Instance.DisplayMessage("Enemy Is Avoidance",NextState);
            }
            else
            {
                if (type == ICharacter.AttackType.Magical)
                {
                    damage -= bossPhase2Status.mDef;
                }
                else
                {
                    damage -= bossPhase2Status.def;
                }
                damage = Mathf.Max(0, damage);
                bossPhase2Status.hp -= damage;
                MessageWindow.Instance.DisplayMessage("Enemy Add Damage" + damage, NextState);
                Debug.Log($"{gameObject.name} は {damage} ダメージを受けた！ 残HP: {bossPhase2Status.hp}");  
            }

            if (bossPhase2Status.hp > 0) return;
        
            StickRotationDetector.Instance.OnRotationCompleted += OnVictoryStickRotate;
            StickRotationDetector.Instance.StartDetection();
        }

        private void OnVictoryStickRotate()
        {
            Debug.Log($"{gameObject.name} を撃破！");
            ResetStatus();
            BattleManager.Instance.SavePlayerInventory();
            MessageWindow.Instance.DisplayMessage("Boss Dead",() =>
            {
                SceneManager.LoadScene("End");
                Destroy(gameObject);
            });
        }

        public void NextState()
        {
            Debug.Log("NextState");
            
            ButtleTurnManager.Instance.ProceedTurn();
        }

        public void ResetStatus()
        {
            bossPhase2Status.hp = bossPhase2Status.maxHp;
            bossPhase2Status.mp = bossPhase2Status.maxMp;
        }
    }
}