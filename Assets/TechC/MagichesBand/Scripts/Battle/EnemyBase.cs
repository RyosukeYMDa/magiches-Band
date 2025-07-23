using Spine.Unity;
using TechC.MagichesBand.Core;
using TechC.MagichesBand.Enemy;
using TechC.MagichesBand.Game;
using TechC.MagichesBand.UI;
using UnityEngine;
using System.Collections;

namespace TechC.MagichesBand.Battle
{
    /// <summary>
    /// 敵キャラクターの共通ベースクラス
    /// 被ダメージ処理、回避、クリティカル判定、死亡演出（スティック回転 or 即死）
    /// </summary>
    public abstract class EnemyBase : MonoBehaviour, ICharacter
    {
        [SerializeField] private CharacterStatus charaStatus; // ステータス情報
        [SerializeField] protected BattlePlayerController battlePlayerController; // プレイヤー操作スクリプトの参照
        [SerializeField] private SkeletonGraphic skeletonGraphic;  // Spineアニメーションの表示用コンポーネント
        [SerializeField] protected bool useStickToDie = true;  // スティック回転を使うかどうか
        
        private const float EvasionRate = 0.1f; //回避率
        private const float CriticalRate = 0.25f; //クリティカルの確率（今は25％）
        private const int CriticalMultiplier = 2; // クリティカル倍率
        private Color originalColor; // 元の色を保持
        
        // ICharacter インターフェース経由でステータス参照用プロパティ
        public CharacterStatus Status => charaStatus;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected virtual void Start()
        {
            originalColor = skeletonGraphic.color;
        }

        /// <summary>
        /// criticalの処理
        /// </summary>
        /// <param name="damage">>通常のダメージ</param>
        /// <returns>クリティカル補正後のダメージ</returns>
        protected int CriticalCalculation(int damage)
        {
            if (!(Random.value < CriticalRate)) return damage;
            
            damage *= CriticalMultiplier;
            Debug.Log("Critical!");
            return damage;
        }
        
        /// <summary>
        /// ダメージを受けたときの処理
        /// </summary>
        public virtual void TakeDamage(int damage, ICharacter.AttackType type)
        {
            // 回避判定（確率で成功）
            if (Random.value < EvasionRate)
            {
                Debug.Log($"回避 残HP: {Status.hp}");
                MessageWindow.Instance.DisplayMessage("回避された", NextState);
                return;
            }

            // 魔法 or 物理によって防御力を適用してダメージ軽減
            damage -= (type == ICharacter.AttackType.Magical) ? Status.mDef : Status.def;
            damage = Mathf.Max(0, damage); // マイナスにはならないよう制限
            Status.hp -= damage;

            // マイナスにはならないよう制限
            if (damage > 0)
            {
                Flash(Color.red, 1f);
                Sound.Instance.Play(SoundType.Damage);
            }

            MessageWindow.Instance.DisplayMessage($"{damage}ダメージを与えた", NextState);
            Debug.Log($"{gameObject.name} は {damage} ダメージを受けた！ 残HP: {Status.hp}");

            if (Status.hp > 0) return;
            
            // 撃破時の処理
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

        /// <summary>
        /// フラッシュ演出を開始
        /// </summary>
        private void Flash(Color color, float duration)
        {
            StartCoroutine(FlashCoroutine(color, duration));
        }

        /// <summary>
        /// 指定色にしてから元の色に戻す演出
        /// </summary>
        private IEnumerator FlashCoroutine(Color color, float duration)
        {
            skeletonGraphic.color = color;
            yield return new WaitForSeconds(duration);
            skeletonGraphic.color = originalColor;
        }

        /// <summary>
        /// ステータスを最大値にリセット
        /// </summary>
        public virtual void ResetStatus()
        {
            Status.hp = Status.maxHp;
            Status.mp = Status.maxMp;
        }

        /// <summary>
        /// 状態遷移処理
        /// </summary>
        public virtual void NextState()
        {
            if (!BattleManager.Instance.playerDead)
                ButtleTurnManager.Instance.ProceedTurn();
        }

        /// <summary>
        /// 撃破時のスティック回転完了後に呼ばれる抽象メソッド
        /// </summary>
        protected abstract void OnVictoryStickRotate();
        
        /// <summary>
        /// 敵の行動処理
        /// </summary>
        public abstract void Act();
    }
}
