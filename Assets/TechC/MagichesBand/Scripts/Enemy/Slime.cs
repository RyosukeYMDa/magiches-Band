using TechC.MagichesBand.Battle;
using TechC.MagichesBand.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using TechC.MagichesBand.Core;

namespace TechC.MagichesBand.Enemy
{
    public class Slime : EnemyBase
    {
        protected override void Start()
        {
            base.Start();
            Sound.Instance.Play(SoundType.SlimeBGM, true);
        }

        public override void Act()
        {
            Debug.Log("EnemyAct");

            if (BattleManager.Instance.enemyDead) return;

            int damage;
            var randomAttack = Random.Range(0, 2);

            switch (randomAttack)
            {
                case 0:
                    Debug.Log("Charge");
                    damage = CriticalCalculation(Mathf.Max(0, Status.atk - battlePlayerController.Status.def));
                    battlePlayerController.TakeDamage(damage, ICharacter.AttackType.Physical);
                    Sound.Instance.Play(SoundType.Charge);
                    break;

                case 1:
                    const int mpCost = 1;
                    if (Status.mp >= mpCost)
                    {
                        Status.mp -= mpCost;
                        Debug.Log("Fire");
                        damage = CriticalCalculation(Mathf.Max(0, Status.mAtk - battlePlayerController.Status.mDef));
                        battlePlayerController.TakeDamage(damage, ICharacter.AttackType.Magical);
                        Sound.Instance.Play(SoundType.Fire);
                    }
                    else
                    {
                        MessageWindow.Instance.DisplayMessage("敵の技が失敗");
                    }
                    break;
            }

            NextState();
        }

        protected override void OnVictoryStickRotate()
        {
            Debug.Log($"{gameObject.name} を撃破！");
            ResetStatus();
            BattleManager.Instance.SavePlayerInventory();
            MessageWindow.Instance.DisplayMessage("Slimeを倒した", () =>
            {
                Sound.Instance.Play(SoundType.Defeated);
                SceneManager.LoadScene("Field");
                Destroy(gameObject);
            });
        }
    }
}