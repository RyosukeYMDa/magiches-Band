using TechC.MagichesBand.Battle;
using TechC.MagichesBand.Game;
using TechC.MagichesBand.UI;
using UnityEngine;

namespace TechC.MagichesBand.Enemy
{
    public class Boss : MonoBehaviour,ICharacter
    {
        [SerializeField] private CharacterStatus bossStatus;
        [SerializeField] private BattlePlayerController battlePlayerController;
    
        private const float CriticalRate = 0.25f; //クリティカルの確率（今は25％）
        private const int CriticalMultiplier = 2; // クリティカル倍率
        private const float EvasionRate = 0.1f; //回避の確率（今は10％）
    
        private int consumptionMp; //消費Mp
        public CharacterStatus Status => bossStatus;
    
        public void Act()
        {
            var damage = 0;
        
            var randomAttack = Random.Range(0, 2);

            switch (randomAttack)
            {
                case 0:
                    Debug.Log("Charge");
                    // プレイヤーへのダメージ計算
                    damage = Mathf.Max(0, bossStatus.atk - battlePlayerController.Status.def);
                    damage = CriticalCalculation(damage);
                    battlePlayerController.TakeDamage(damage, ICharacter.AttackType.Physical);
                    break;
                case 1:
                    consumptionMp = 1;
                    if ((bossStatus.mp - consumptionMp) >= 0)
                    {
                        Debug.Log("Fire");
                        // プレイヤーへのダメージ計算
                        damage = Mathf.Max(0, bossStatus.mAtk - battlePlayerController.Status.mDef);
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
                Debug.Log($"回避  残HP: {bossStatus.hp}");
                MessageWindow.Instance.DisplayMessage("Enemy Is Avoidance",NextState);
            }
            else
            {
                if (type == ICharacter.AttackType.Magical)
                {
                    damage -= bossStatus.mDef;
                }
                else
                {
                    damage -= bossStatus.def;
                }
                damage = Mathf.Max(0, damage);
                bossStatus.hp -= damage;
                MessageWindow.Instance.DisplayMessage("Enemy Add Damage" + damage,NextState);
                Debug.Log($"{gameObject.name} は {damage} ダメージを受けた！ 残HP: {bossStatus.hp}"); 
            }

            if (bossStatus.hp <= 0)
            {
                if (!BattleManager.Instance.enemyDead)
                {
                    BattleManager.Instance.enemyDead = true;

                    Debug.Log($"{gameObject.name} を撃破！");
                    ResetStatus();
                    MessageWindow.Instance.DisplayMessage("Boss Mutation", () =>
                    {
                        BattleManager.Instance.SpawnPhase2Boss();
                        // 現在のボスを削除して2段階目を生成
                        Destroy(gameObject);
                    });
                }
            }
        }

        public void NextState()
        {
            if(BattleManager.Instance.enemyDead)return;
        
            ButtleTurnManager.Instance.ProceedTurn();
        }
    
        public void ResetStatus()
        {
            bossStatus.hp = bossStatus.maxHp;
            bossStatus.mp = bossStatus.maxMp;
        }
    }
}
