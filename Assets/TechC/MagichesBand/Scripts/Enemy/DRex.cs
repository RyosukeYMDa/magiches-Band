using TechC.MagichesBand.Battle;
using TechC.MagichesBand.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using TechC.MagichesBand.Core;

namespace TechC.MagichesBand.Enemy
{
    /// <summary>
    /// DRexのパラメーターや行動を管理するclass
    /// </summary>
    public class DRex : EnemyBase
    {
        //キャッシュ
        private Sound sound;
        private BattleManager battleManager;
        private MessageWindow messageWindow;

        protected override void Awake()
        {
            //キャッシュ
            base.Awake();
            sound = Sound.Instance; 
            battleManager = BattleManager.Instance;
            messageWindow = MessageWindow.Instance;
        }
        
        protected override void Start()
        {
            base.Start();
            Sound.Instance.Play(SoundType.DragonBGM, true);
        }

        /// <summary>
        /// DRexの行動処理
        /// ランダムで攻撃方法を選びプレイヤーにダメージを与える
        /// </summary>
        public override void Act()
        {
            Debug.Log("EnemyAct");

            if (BattleManager.Instance.enemyDead) return;

            int damage;
            var randomAttack = Random.Range(0, 2);

            switch (randomAttack)
            {
                case 0:
                    Debug.Log("Bite");
                    damage = CriticalCalculation(Mathf.Max(0, Status.atk - battlePlayerController.Status.def));
                    battlePlayerController.TakeDamage(damage, ICharacter.AttackType.Physical);
                    Sound.Instance.Play(SoundType.Bite);
                    break;

                case 1:
                    const int mpCost = 3;
                    if (Status.mp >= mpCost)
                    {
                        Status.mp -= mpCost;
                        Debug.Log("Breath");
                        damage = CriticalCalculation(Mathf.Max(0, Status.mAtk - battlePlayerController.Status.mDef));
                        battlePlayerController.TakeDamage(damage, ICharacter.AttackType.Magical);
                        Sound.Instance.Play(SoundType.Breath);
                    }
                    else
                    {
                        MessageWindow.Instance.DisplayMessage("敵の技が失敗");
                    }
                    break;
            }

            NextState();
        }

        /// <summary>
        /// 敵撃破後の処理
        /// スティック回転による勝利演出後にシーンを移動する
        /// </summary>
        protected override void OnVictoryStickRotate()
        {
            Debug.Log($"{gameObject.name} を撃破！");
            ResetStatus();
            BattleManager.Instance.SavePlayerInventory();
            MessageWindow.Instance.DisplayMessage("DRexを倒した", () =>
            {
                Sound.Instance.Play(SoundType.Defeated);
                SceneManager.LoadScene("Field");
                Destroy(gameObject);
            });
        }
    }
}
