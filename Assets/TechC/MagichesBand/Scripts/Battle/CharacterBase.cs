using Spine.Unity;
using TechC.MagichesBand.Core;
using TechC.MagichesBand.Enemy;
using TechC.MagichesBand.Game;
using TechC.MagichesBand.UI;
using UnityEngine;
using System.Collections;

namespace TechC.MagichesBand.Battle
{
    public abstract class CharacterBase : MonoBehaviour, ICharacter
    {
        [SerializeField] private CharacterStatus charaStatus;
        [SerializeField] protected BattlePlayerController battlePlayerController;
        [SerializeField] private SkeletonGraphic skeletonGraphic;
        [SerializeField] protected bool useStickToDie = true;
        
        private const float CriticalRate = 0.25f; //クリティカルの確率（今は25％）
        private const int CriticalMultiplier = 2; // クリティカル倍率
        private const float EvasionRate = 0.1f; //回避の確率（今は10％）
        private Color originalColor;
        
        public CharacterStatus Status => charaStatus;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected virtual void Start()
        {
            originalColor = skeletonGraphic.color;
        }

        /// <summary>
        /// criticalの処理
        /// </summary>
        /// <param name="damage"></param>
        protected int CriticalCalculation(int damage)
        {
            if (!(Random.value < CriticalRate)) return damage;
            
            damage *= CriticalMultiplier;
            Debug.Log("Critical!");
            return damage;
        }
        
        public virtual void TakeDamage(int damage, ICharacter.AttackType type)
        {
            if (Random.value < 0.1f)
            {
                Debug.Log($"回避 残HP: {Status.hp}");
                MessageWindow.Instance.DisplayMessage("Enemy Is Avoidance", NextState);
                return;
            }

            damage -= (type == ICharacter.AttackType.Magical) ? Status.mDef : Status.def;
            damage = Mathf.Max(0, damage);
            Status.hp -= damage;

            if (damage > 0)
            {
                Flash(Color.red, 1f);
                Sound.Instance.Play(SoundType.Damage);
            }

            MessageWindow.Instance.DisplayMessage("Enemy Add Damage" + damage, NextState);
            Debug.Log($"{gameObject.name} は {damage} ダメージを受けた！ 残HP: {Status.hp}");

            if (Status.hp <= 0)
            {
                if (useStickToDie)
                {
                    StickRotationDetector.Instance.OnRotationCompleted += OnVictoryStickRotate;
                    StickRotationDetector.Instance.StartDetection();
                }
                else
                {
                    OnVictoryStickRotate();
                }
            }
        }

        protected void Flash(Color color, float duration)
        {
            StartCoroutine(FlashCoroutine(color, duration));
        }

        private IEnumerator FlashCoroutine(Color color, float duration)
        {
            skeletonGraphic.color = color;
            yield return new WaitForSeconds(duration);
            skeletonGraphic.color = originalColor;
        }

        public virtual void ResetStatus()
        {
            Status.hp = Status.maxHp;
            Status.mp = Status.maxMp;
        }

        public virtual void NextState()
        {
            if (!BattleManager.Instance.playerDead)
                ButtleTurnManager.Instance.ProceedTurn();
        }

        protected abstract void OnVictoryStickRotate();
        public abstract void Act();
    }
}
