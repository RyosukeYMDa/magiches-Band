using TechC.MagichesBand.Battle;
using TechC.MagichesBand.Game;
using TechC.MagichesBand.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Spine.Unity;
using System.Collections;
using TechC.MagichesBand.Core;

namespace TechC.MagichesBand.Enemy
{
    public class Dragon : CharacterBase
    {
        protected override void Start()
        {
            base.Start();
            Sound.Instance.Play(SoundType.DragonBGM, true);
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
                        MessageWindow.Instance.DisplayMessage("SkillFailed");
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
            MessageWindow.Instance.DisplayMessage("Dragon", () =>
            {
                Sound.Instance.Play(SoundType.Defeated);
                SceneManager.LoadScene("Field");
                Destroy(gameObject);
            });
        }
    }
}
