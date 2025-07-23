using TechC.MagichesBand.Battle;
using TechC.MagichesBand.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using TechC.MagichesBand.Core;

namespace TechC.MagichesBand.Enemy
{
    /// <summary>
    /// BossPhase2のパラメーターや行動を管理するclass
    /// </summary>
    public class BossPhase2 : EnemyBase
      {
        protected override void Start()
        {
            base.Start();
            Sound.Instance.Play(SoundType.Boss2BGM, true);
        }

        /// <summary>
        /// BossPhase2の行動処理
        /// ランダムで攻撃方法を選びプレイヤーにダメージを与える
        /// </summary>
        public override void Act()
        {
            if (BattleManager.Instance.enemyDead) return;

            int damage;
            var randomAttack = Random.Range(0, 3);

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
                        MessageWindow.Instance.DisplayMessage("敵の技が失敗");
                    }
                    break;

                case 2:
                    const int mpCostBuffNegate = 1;
                    if (Status.mp >= mpCostBuffNegate)
                    {
                        Status.mp -= mpCostBuffNegate;
                        Debug.Log("NegationBuff");
                        MessageWindow.Instance.DisplayMessage("上昇効果が消えた");
                        battlePlayerController.ResetBuff();
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
            MessageWindow.Instance.DisplayMessage("Bossを倒した", () =>
            {
                Sound.Instance.Play(SoundType.Defeated);
                SceneManager.LoadScene("End");
                Destroy(gameObject);
            });
        }
    }
}