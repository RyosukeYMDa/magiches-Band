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
    /// <summary>
    /// バトル中のプレイヤー操作を担当するクラス
    /// コマンド選択や攻撃処理 ダメージ処理などを制御する
    /// </summary>
    public class BattlePlayerController : MonoBehaviour,ICharacter
    {
        [SerializeField] private CharacterStatus playerStatus;
        [SerializeField] private GameObject attackCommand;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private InventoryUI inventoryUI;
        [SerializeField] private BattleManager battleManager;
        [SerializeField] private ItemSelect itemSelect;
        [SerializeField] private GameObject actButton;
        
        // クリティカルや回避 バフの設定値
        private const float CriticalRate = 0.25f; //クリティカルの確率（今は25％）
        private const int CriticalMultiplier = 2; // クリティカル倍率
        private const float EvasionRate = 0.1f; //回避の確率（今は10％）
        private const int MaxAtkBuffValue = 16; //攻撃力バフの上限
        private const int MaxDefBuffValue = 16; //防御力バフの上限
        
        
        private int consumptionMp; //消費Mp

        public int atkDoublingValue; //攻撃上昇補正
        public int defDoublingValue; //防御上昇補正

        private void Update()
        {
            // アイテム選択が完了したかを監視し 状態をリセットしてターンを進める
            if (!inventoryUI.isInventory || !inventoryUI.isItem) return;
        
            inventoryUI.isItem = false;
            itemSelect.contentParent.gameObject.SetActive(false);
            inventoryUI.SetInventoryState(false);
            NextState();
        }   
    
        /// <summary>
        /// プレイヤーの行動開始処理 ボタンを有効化する
        /// </summary>
        public void Act()
        {
            Debug.Log("NotButtonAct");
            
            if (BattleManager.Instance.playerDead || BattleManager.Instance.enemyDead) return;
            
            Debug.Log("ButtonAct");
            BattleManager.Instance.EnableActButton();
        }
    
        /// <summary>
        /// 魔法攻撃のコマンド MPを消費し敵にダメージを与える
        /// </summary>
        public void Explosion()
        {
            consumptionMp = 4;
            if ((playerStatus.mp - consumptionMp) >= 0)
            {
                Debug.Log("Explosion");
             
                Sound.Instance.Play(SoundType.Explosion);
                messageText.gameObject.SetActive(false);
            
                attackCommand.SetActive(false);
        
                // 敵を取得
                var enemy = BattleManager.Instance.CurrentEnemy;
        
                // ダメージ計算
                var damage = Mathf.Max(0, playerStatus.mAtk + atkDoublingValue);
                damage = CriticalCalculation(damage, ICharacter.AttackType.Magical);
            }
            else
            {
                MessageWindow.Instance.DisplayMessage("技が失敗した", NextState);
            }
        }

        /// <summary>
        /// 物理攻撃のコマンド 敵にダメージを与える
        /// </summary>
        public void Shoot()
        {
            Debug.Log("Slash");
            
            Sound.Instance.Play(SoundType.Shoot);
            messageText.gameObject.SetActive(false);
        
            attackCommand.SetActive(false);
        
            // 敵を取得
            var enemy = BattleManager.Instance.CurrentEnemy;
        
            // ダメージ計算
            var damage = Mathf.Max(0,playerStatus.atk + atkDoublingValue);
            damage = CriticalCalculation(damage, ICharacter.AttackType.Physical);
        }

        /// <summary>
        /// 攻撃力を上昇させる バフの上限を超えないように制御
        /// </summary>
        public void AtkUp()
        {
            switch (atkDoublingValue)
            {
                case MaxAtkBuffValue:
                    attackCommand.SetActive(false);
                    MessageWindow.Instance.DisplayMessage("これ以上攻撃力は上がらない", NextState);
                    return;
                case 0:
                    atkDoublingValue　= (atkDoublingValue + 1) * 2;
                    break;
                default:
                    atkDoublingValue *= 2;
                    break;
            }

            Debug.Log(atkDoublingValue);
            
            BattleManager.Instance.BuffEffect();
            Sound.Instance.Play(SoundType.StatusUp);
        
            messageText.gameObject.SetActive(false);
            attackCommand.SetActive(false);
            
            MessageWindow.Instance.DisplayMessage("攻撃力が少し上がった", NextState);
        }

        /// <summary>
        /// 防御力を上昇させる バフの上限を超えないように制御
        /// </summary>
        public void DefUp()
        {
            switch (defDoublingValue)
            {
                case MaxDefBuffValue:
                    attackCommand.SetActive(false);
                    MessageWindow.Instance.DisplayMessage("これ以上防御力は上がらない", NextState);
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
            Sound.Instance.Play(SoundType.StatusUp);
            
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
            var enemy = BattleManager.Instance.CurrentEnemy;
            
            // ランダム値を生成
            var randomCritical = Random.Range(0.0f, 1.0f);
        
            //ランダム値よりクリティカル確率が上だったら、クリティカルがでる
            if (randomCritical < CriticalRate)
            {
                damage *= CriticalMultiplier;
                MessageWindow.Instance.DisplayMessage("クリティカルを出した", () =>
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
                MessageWindow.Instance.DisplayMessage("回避した");
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
                    Sound.Instance.Play(SoundType.Damage);
                }
                MessageWindow.Instance.DisplayMessage($"{damage}ダメージを受けた");
            }

            if (playerStatus.hp > 0) return;
            
            // プレイヤー撃破時の処理
            BattleManager.Instance.playerDead = true;
            ResetStatus();
            GameManager.Instance.playerPosition = new Vector3(-13f, 0.6f, 6);
            enemy.ResetStatus();
            MessageWindow.Instance.DisplayMessage("Playerが倒された", () =>
            {
                Sound.Instance.Play(SoundType.Defeated);
                SceneManager.LoadScene("Title");
            });
        }

        /// <summary>
        /// 次のターンへ進む
        /// </summary>
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
    
        /// <summary>
        /// プレイヤーのHPとMPを最大値にリセットする
        /// </summary>
        public void ResetStatus()
        {
            playerStatus.hp = playerStatus.maxHp;
            playerStatus.mp = playerStatus.maxMp;
        }

        /// <summary>
        /// 攻撃と防御のバフをリセットする
        /// </summary>
        public void ResetBuff()
        {
            atkDoublingValue = 0;
            defDoublingValue = 0;
        }
    
        // ICharacterのStatusプロパティ
        public CharacterStatus Status => playerStatus;
    }
}
