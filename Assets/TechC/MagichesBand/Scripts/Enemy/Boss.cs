using TechC.MagichesBand.Battle;
using TechC.MagichesBand.UI;
using UnityEngine;
using TechC.MagichesBand.Core;

namespace TechC.MagichesBand.Enemy
{
    public class Boss : CharacterBase
    {
        private const int ConsumptionMp = 1;

        protected override void Start()
        {
            base.Start();
            useStickToDie = false; // BossはStick回転しない
            Sound.Instance.Play(SoundType.BossBGM, true);
        }

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
                MessageWindow.Instance.DisplayMessage("SkillFailed");
            }

            NextState();
        }

        protected override void OnVictoryStickRotate()
        {
            Debug.Log($"{gameObject.name} を撃破！");
            ResetStatus();

            MessageWindow.Instance.DisplayMessage("Boss", () =>
            {
                BattleManager.Instance.SpawnPhase2Boss();
                Sound.Instance.Play(SoundType.Defeated);
                BattleManager.Instance.enemyDead = false; // Phase2の行動が止まらないように
                Destroy(gameObject);
            });
        }
    }
}
