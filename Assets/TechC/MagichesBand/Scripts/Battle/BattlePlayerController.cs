using System;
using TechC.MagichesBand.Core;
using TechC.MagichesBand.Enemy;
using TechC.MagichesBand.Game;
using TechC.MagichesBand.Item;
using TechC.MagichesBand.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace TechC.MagichesBand.Battle
{
    public class BattlePlayerController : MonoBehaviour,ICharacter
    {
        [SerializeField] private CharacterStatus playerStatus;
        [SerializeField] private GameObject attackCommand;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private InventoryUI inventoryUI;
        [SerializeField] private BattleManager battleManager;
        [SerializeField] private ItemSelect itemSelect;
        [SerializeField] private GameObject actButton;
        
        private const float CriticalRate = 0.25f; //クリティカルの確率（今は25％）
        private const int CriticalMultiplier = 2; // クリティカル倍率
        private const float EvasionRate = 0.1f; //回避の確率（今は10％）
    
        private int consumptionMp; //消費Mp

        public int atkDoublingValue; //攻撃上昇補正
        public int defDoublingValue; //防御上昇補正

        private void Update()
        {
            if (!inventoryUI.isInventory || !inventoryUI.isItem) return;
        
            inventoryUI.isItem = false;
            itemSelect.contentParent.gameObject.SetActive(false);
            inventoryUI.SetInventoryState(false);
            NextState();
        }   
    
        public void Act()
        {
            Debug.Log("NotButtonAct");
            
            if (BattleManager.Instance.playerDead || BattleManager.Instance.enemyDead) return;
            
            Debug.Log("ButtonAct");
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
                var damage = Mathf.Max(0, playerStatus.mAtk + atkDoublingValue);
                damage = CriticalCalculation(damage, ICharacter.AttackType.Magical);
            }
            else
            {
                MessageWindow.Instance.DisplayMessage("Failure", NextState);
            }
        }

        public void Shoot()
        {
            Debug.Log("Slash");
            messageText.gameObject.SetActive(false);
        
            attackCommand.SetActive(false);
        
            ICharacter enemy = BattleManager.Instance.CurrentEnemy;
        
            int damage = Mathf.Max(0,playerStatus.atk + atkDoublingValue);
            damage = CriticalCalculation(damage, ICharacter.AttackType.Physical);
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
            
            BattleManager.Instance.BuffEffect();
            Sound.Instance.Play(SoundType.AtkUp);
        
            messageText.gameObject.SetActive(false);
            attackCommand.SetActive(false);
            
            MessageWindow.Instance.DisplayMessage("攻撃力が少し上がった", NextState);
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
        
            BattleManager.Instance.BuffEffect();
            
            messageText.gameObject.SetActive(false);
            attackCommand.SetActive(false);
        
            MessageWindow.Instance.DisplayMessage("防御力が少し上がった", NextState);
        }

        /// <summary>
        /// criticalの処理（クリティカルの確率を個別に変えたいのでまとめておかない）
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="type"></param>
        private int CriticalCalculation(int damage, ICharacter.AttackType type)
        {
            ICharacter enemy = BattleManager.Instance.CurrentEnemy;
            
            // ランダム値を生成
            float randomCritical = Random.Range(0.0f, 1.0f);
        
            //ランダム値よりクリティカル確率が上だったら、クリティカルがでる
            if (randomCritical < CriticalRate)
            {
                damage *= CriticalMultiplier;
                MessageWindow.Instance.DisplayMessage("Player Is Critical", () =>
                {
                    enemy.TakeDamage(damage, type);
                });
                Debug.Log("Critical");
            }
            else
            {
                enemy.TakeDamage(damage, type);
            }
            return damage;
        }


        /// <summary>
        /// 相手からダメージを受け取り、確率で回避をさせる
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="type"></param>
        public void TakeDamage(int damage, ICharacter.AttackType type)
        {
            var enemy = BattleManager.Instance.CurrentEnemy;
        
            var randomEvasion = Random.Range(0.0f, 1.0f);

            if (randomEvasion < EvasionRate)
            {
                Debug.Log($"回避  残HP: {playerStatus.hp}");
                MessageWindow.Instance.DisplayMessage("Player Is Avoidance");
            }
            else
            {
                if (type == ICharacter.AttackType.Magical)
                {
                    Debug.Log("player mDef");
                    
                    damage -= playerStatus.mDef;
                }
                else
                {
                    Debug.Log("player Def");
                    
                    damage -= playerStatus.def;
                }
                damage = Mathf.Max(0, damage);
                playerStatus.hp -= damage;
                if (damage > 0)
                {
                    BattleManager.Instance.DamageEffect();
                }
                MessageWindow.Instance.DisplayMessage("Player Is Hit" + damage);
            }

            if (playerStatus.hp <= 0)
            {
                BattleManager.Instance.playerDead = true;
                ResetStatus();
                GameManager.Instance.playerPosition = new Vector3(-13f, 0.6f, 6);
                enemy.ResetStatus();
                MessageWindow.Instance.DisplayMessage("Player Dead", () =>
                {
                    SceneManager.LoadScene("Title");
                });
            }
        }

        public void NextState()
        {
            if (BattleManager.Instance.playerDead)
            {
                Debug.Log("playerDead");
                actButton.SetActive(false);
                return;
            }
            
            ButtleTurnManager.Instance.ProceedTurn();
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
