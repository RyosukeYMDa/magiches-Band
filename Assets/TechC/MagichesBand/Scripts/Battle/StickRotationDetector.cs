using System.Collections.Generic;
using TechC.MagichesBand.Core;
using TechC.MagichesBand.UI;
using UnityEngine;

namespace TechC.MagichesBand.Battle
{
    /// <summary>
    /// バトルにおけるスティック回転入力の判定オブジェクト
    /// 倒された敵に対して、左スティックを一周回すことで処理を進める
    /// </summary>
    public class StickRotationDetector : SingletonMonoBehaviour<StickRotationDetector>
    {
        [SerializeField] private float directionDeadZone = 0.5f;  // スティック入力の無視する値（スティックの傾きがこの値未満なら無視）
        [SerializeField] private float directionChangeAngle = 45f; // 1方向を何度の範囲で定義するか
        [SerializeField] private int rotationThreshold = 8; // 1周分とみなすために必要な方向の数

        private readonly List<int> directionHistory = new List<int>();  // 入力された方向の履歴
        private int lastDirection = -1; // 前回の方向インデックス

        public System.Action OnRotationCompleted; // スティックを1周したときに呼び出されるイベント
    
        public bool defeatedEnemy; //敵に倒されているかの判別
    
        private void Update()
        {
            if (!defeatedEnemy) return;

            // 左スティックの入力値を取得
            var stick = UnityEngine.InputSystem.Gamepad.current?.leftStick.ReadValue() ?? Vector2.zero;

            // 入力が小さすぎる場合は無視
            if (stick.magnitude < directionDeadZone) return;

            // スティックの角度を取得
            var angle = Mathf.Atan2(stick.y, stick.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360;

            // 方向インデックスを計算
            var directionIndex = Mathf.FloorToInt(angle / directionChangeAngle) % 8;

            // 同じ方向が続いた場合は履歴に追加しない
            if (directionIndex == lastDirection) return;
            
            // 新しい方向を履歴に追加し、最後の方向を更新
            directionHistory.Add(directionIndex);
            lastDirection = directionIndex;

            // 履歴がrotationThresholdに達したら1周とみなして処理を進める
            if (directionHistory.Count < rotationThreshold) return;
            Debug.Log("スティック1周完了！");
            
            if (OnRotationCompleted != null)
            {
                Debug.Log("Invoke実行します");
                OnRotationCompleted.Invoke();
            }
            else
            {
                Debug.LogWarning("Invoke先がnullです！");
            }
            // 状態リセット
            directionHistory.Clear();
            defeatedEnemy = false;
        }

        /// <summary>
        /// 回転検出を開始する（UIにメッセージ表示なども含む）
        /// </summary>
        public void StartDetection()
        {
            Debug.Log("StartDetection");
            
            BattleManager.Instance.enemyDead = true;
            
            MessageWindow.Instance.DisplayMessage("左Stickを回せ", () =>
            {
                directionHistory.Clear(); // 履歴をリセット
                lastDirection = -1;
                defeatedEnemy = true; // 検出を開始
            });
        }
    }
}
