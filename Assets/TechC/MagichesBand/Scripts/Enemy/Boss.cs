using TechC.MagichesBand.Battle;
using TechC.MagichesBand.UI;
using UnityEngine;
using TechC.MagichesBand.Core;

namespace TechC.MagichesBand.Enemy
{
    /// <summary>
    /// Bossのパラメーターや行動を管理するclass
    /// </summary>
    public class Boss : EnemyBase
    {
        private const int ConsumptionMp = 1;

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
            useStickToDie = false; // BossはStick回転しない
            Sound.Instance.Play(SoundType.BossBGM, true);
        }

        /// <summary>
        /// Bossの行動処理
        /// ランダムで攻撃方法を選びプレイヤーにダメージを与える
        /// </summary>
        public override void Act()
        {
            var damage = 0;
            var choice = Random.Range(0, 2);

            if (choice == 0)
            {
                Debug.Log("Rain");
                damage = CriticalCalculation(Mathf.Max(0, Status.atk - battlePlayerController.Status.def));
                battlePlayerController.TakeDamage(damage, ICharacter.AttackType.Physical);
                Sound.Instance.Play(SoundType.Rain);
            }
            else if (Status.mp >= ConsumptionMp)
            {
                Debug.Log("Electricity");
                Status.mp -= ConsumptionMp;
                damage = CriticalCalculation(Mathf.Max(0, Status.mAtk - battlePlayerController.Status.mDef));
                battlePlayerController.TakeDamage(damage, ICharacter.AttackType.Magical);
                Sound.Instance.Play(SoundType.Electricity);
            }
            else
            {
                MessageWindow.Instance.DisplayMessage("敵の技が失敗");
            }

            NextState();
        }

        /// <summary>
        /// 敵撃破後の処理
        /// </summary>
        protected override void OnVictoryStickRotate()
        {
            Debug.Log($"{gameObject.name} を撃破！");
            ResetStatus();

            MessageWindow.Instance.DisplayMessage("Bossの様子がおかしい", () =>
            {
                BattleManager.Instance.SpawnPhase2Boss();
                Sound.Instance.Play(SoundType.Defeated);
                BattleManager.Instance.enemyDead = false; // Phase2の行動が止まらないように
                Destroy(gameObject);
            });
        }
    }
}
