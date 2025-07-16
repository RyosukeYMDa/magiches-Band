using System.Collections.Generic;
using TechC.MagichesBand.Core;
using TechC.MagichesBand.UI;
using UnityEngine;

namespace TechC.MagichesBand.Battle
{
    /// <summary>
    ///     バトルにおけるスティック回転入力の判定オブジェクト
    /// </summary>
    public class StickRotationDetector : SingletonMonoBehaviour<StickRotationDetector>
    {
        [SerializeField] private float directionDeadZone = 0.5f;
        [SerializeField] private float directionChangeAngle = 45f;
        [SerializeField] private int rotationThreshold = 8;

        private readonly List<int> directionHistory = new List<int>();
        private int lastDirection = -1;

        public System.Action OnRotationCompleted;
    
        public bool defeatedEnemy; //倒されているかの判別
    
        private void Update()
        {
            if (!defeatedEnemy) return;

            var stick = UnityEngine.InputSystem.Gamepad.current?.leftStick.ReadValue() ?? Vector2.zero;

            if (stick.magnitude < directionDeadZone) return;

            var angle = Mathf.Atan2(stick.y, stick.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360;

            var directionIndex = Mathf.FloorToInt(angle / directionChangeAngle) % 8;

            if (directionIndex == lastDirection) return;
            directionHistory.Add(directionIndex);
            lastDirection = directionIndex;

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
            directionHistory.Clear();
            defeatedEnemy = false;
        }

        public void StartDetection()
        {
            Debug.Log("StartDetection");
            BattleManager.Instance.enemyDead = true;
            MessageWindow.Instance.DisplayMessage("Turn The Stick", () =>
            {
                directionHistory.Clear();
                lastDirection = -1;
                defeatedEnemy = true;
            });
        }
    }
}
