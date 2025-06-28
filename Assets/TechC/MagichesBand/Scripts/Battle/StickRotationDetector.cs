using System.Collections.Generic;
using UnityEngine;

namespace TechC.MagichesBand.Battle
{
    public class StickRotationDetector : MonoBehaviour
    {
        [SerializeField] private float directionDeadZone = 0.5f;
        [SerializeField] private float directionChangeAngle = 45f;
        [SerializeField] private int rotationThreshold = 8;

        private List<int> directionHistory = new List<int>();
        private int lastDirection = -1;

        public System.Action OnRotationCompleted;
    
        public bool defeatedEnemy; //倒されているかの判別

        public static StickRotationDetector Instance { get; private set; }

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
    
        private void Update()
        {
            if (!defeatedEnemy) return;

            Vector2 stick = UnityEngine.InputSystem.Gamepad.current?.leftStick.ReadValue() ?? Vector2.zero;

            if (stick.magnitude < directionDeadZone) return;

            float angle = Mathf.Atan2(stick.y, stick.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360;

            int directionIndex = Mathf.FloorToInt(angle / directionChangeAngle) % 8;

            if (directionIndex != lastDirection)
            {
                directionHistory.Add(directionIndex);
                lastDirection = directionIndex;

                if (directionHistory.Count >= rotationThreshold)
                {
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
            }
        }

        public void StartDetection()
        {
            Debug.Log("StartDetection");
            directionHistory.Clear();
            lastDirection = -1;
            defeatedEnemy = true;
        }
    }
}
