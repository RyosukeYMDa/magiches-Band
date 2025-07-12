using TechC.MagichesBand.Battle;
using TechC.MagichesBand.Game;
using TechC.MagichesBand.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Spine.Unity;
using System.Collections;

namespace TechC.MagichesBand.Enemy
{
    public class Slime : MonoBehaviour,ICharacter
    {
        [SerializeField] private CharacterStatus slimeStatus;
        [SerializeField] private BattlePlayerController battlePlayerController;
    
        private const float CriticalRate = 0.25f; //クリティカルの確率（今は25％）
        private const int CriticalMultiplier = 2; // クリティカル倍率
        private const float EvasionRate = 0.1f; //回避の確率（今は10％）
    
        private int consumptionMp; //消費Mp
        
        [SerializeField] SkeletonGraphic skeletonGraphic;
        private Color originalColor;
        
        public CharacterStatus Status => slimeStatus;
    
        private void Start()
        {
            // 初期色を保存
            originalColor = skeletonGraphic.color;
        }
        
        public void Act()
        {
            Debug.Log("EnemyAct");
            
            if(StickRotationDetector.Instance.defeatedEnemy) return;
        
            var damage = 0;
        
            var randomAttack = Random.Range(0, 2);

            switch (randomAttack)
            {
                case 0:
                    Debug.Log("Charge");
                    // プレイヤーへのダメージ計算
                    damage = Mathf.Max(0, slimeStatus.atk - battlePlayerController.Status.def);
                    damage = CriticalCalculation(damage);
                    battlePlayerController.TakeDamage(damage, ICharacter.AttackType.Physical);
                    break;
                case 1:
                    consumptionMp = 1;
                    if ((slimeStatus.mp - consumptionMp) >= 0)
                    {
                        Debug.Log("Fire");
                        slimeStatus.mp -= consumptionMp;
                        // プレイヤーへのダメージ計算
                        damage = Mathf.Max(0, slimeStatus.mAtk - battlePlayerController.Status.mDef);
                        damage = CriticalCalculation(damage);
                        battlePlayerController.TakeDamage(damage, ICharacter.AttackType.Magical);   
                    }
                    else
                    {
                        MessageWindow.Instance.DisplayMessage("SkillFailed");
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
                Debug.Log($"回避  残HP: {slimeStatus.hp}");
                MessageWindow.Instance.DisplayMessage("Enemy Is Avoidance",NextState);
                
            }
            else
            {
                if (type == ICharacter.AttackType.Magical)
                {
                    damage -= slimeStatus.mDef;
                }
                else
                {
                    damage -= slimeStatus.def;
                }
                damage = Mathf.Max(0, damage);
                slimeStatus.hp -= damage;
                if (damage > 0)
                {
                    Flash(Color.red, 1f);
                }
                MessageWindow.Instance.DisplayMessage("Enemy Add Damage" + damage, NextState);
                Debug.Log($"{gameObject.name} は {damage} ダメージを受けた！ 残HP: {slimeStatus.hp}");
            }

            if (slimeStatus.hp > 0) return;
        
            StickRotationDetector.Instance.OnRotationCompleted += OnVictoryStickRotate;
            StickRotationDetector.Instance.StartDetection();
        }
        
        private void Flash(Color color, float duration)
        {
            StartCoroutine(FlashCoroutine(color, duration));
        }

        private IEnumerator FlashCoroutine(Color color, float duration)
        {
            skeletonGraphic.color = color; // 色を赤などに変更
            yield return new WaitForSeconds(duration);
            skeletonGraphic.color = originalColor; // 元の色に戻す
        }

        private void OnVictoryStickRotate()
        {
            Debug.Log($"{gameObject.name} を撃破！");
            ResetStatus();
            BattleManager.Instance.SavePlayerInventory();
            MessageWindow.Instance.DisplayMessage("Slime Dead", () =>
            {
                SceneManager.LoadScene("Field");
                Destroy(gameObject); 
            });
        }

        public void NextState()
        {
            if(BattleManager.Instance.playerDead)return;
            
            Debug.Log("NextState");
            
            ButtleTurnManager.Instance.ProceedTurn();
        }

        public void ResetStatus()
        {
            slimeStatus.hp = slimeStatus.maxHp;
            slimeStatus.mp = slimeStatus.maxMp;
        }
    }
}