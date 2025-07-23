using TechC.MagichesBand.Battle;
using TechC.MagichesBand.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using TechC.MagichesBand.Core;

namespace TechC.MagichesBand.Enemy
{
    /// <summary>
    /// Slimeのパラメーターや行動を管理するclass
    /// </summary>
    public class Slime : EnemyBase
    {
        protected override void Start()
        {
            base.Start();
            Sound.Instance.Play(SoundType.SlimeBGM, true);
        }

        /// <summary>
        /// スライムの行動処理
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

        /// <summary>
        /// 敵撃破後の処理
        /// スティック回転による勝利演出後にシーンを移動する
        /// </summary>
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