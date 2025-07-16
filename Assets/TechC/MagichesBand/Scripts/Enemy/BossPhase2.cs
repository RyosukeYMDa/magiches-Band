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
    public class BossPhase2 : CharacterBase
      {
        protected override void Start()
        {
            base.Start();
            Sound.Instance.Play(SoundType.Boss2BGM, true);
        }

        public override void Act()
        {
            if (BattleManager.Instance.enemyDead) return;

            int damage;
            int randomAttack = Random.Range(0, 3);

            switch (randomAttack)
            {
                case 0:
                    Debug.Log("Thunder");
                    damage = CriticalCalculation(Mathf.Max(0, Status.atk - battlePlayerController.Status.def));
                    battlePlayerController.TakeDamage(damage, ICharacter.AttackType.Physical);
                    Sound.Instance.Play(SoundType.Thunder);
                    break;

                case 1:
                    const int mpCostBlackHole = 1;
                    if (Status.mp >= mpCostBlackHole)
                    {
                        Status.mp -= mpCostBlackHole;
                        Debug.Log("Black hole");
                        damage = CriticalCalculation(Mathf.Max(0, Status.mAtk - battlePlayerController.Status.mDef));
                        battlePlayerController.TakeDamage(damage, ICharacter.AttackType.Magical);
                        Sound.Instance.Play(SoundType.BlackHole);
                    }
                    else
                    {
                        MessageWindow.Instance.DisplayMessage("SkillFailed");
                    }
                    break;

                case 2:
                    const int mpCostBuffNegate = 1;
                    if (Status.mp >= mpCostBuffNegate)
                    {
                        Status.mp -= mpCostBuffNegate;
                        Debug.Log("NegationBuff");
                        MessageWindow.Instance.DisplayMessage("NegationBuff");
                        battlePlayerController.ResetBuff();
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
            MessageWindow.Instance.DisplayMessage("Boss Dead", () =>
            {
                Sound.Instance.Play(SoundType.Defeated);
                SceneManager.LoadScene("End");
                Destroy(gameObject);
            });
        }
    }
}