using TechC.MagichesBand.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TechC.MagichesBand.UI
{
    /// <summary>
    /// 複数ボタンをGamepad操作でナビゲートするクラス
    /// </summary>
    public class ButtonNavigator : MonoBehaviour
    {
    
        //ボタンを格納
        public Button[] buttons;
        private int currentIndex;
        
        // 左スティックによる操作を有効とみなす閾値
        private const float StickThreshold = 0.5f;
        
        private Sound sound;

        private void Awake()
        {
            sound = Sound.Instance;
        }
        
        private void Start()
        {
            currentIndex = 0;
            
            // 最初のボタンを選択状態にする
            SelectButton(currentIndex,playSound:false);
        }

        private void Update()
        {
            var upPressed = false;
            var downPressed = false;

            // Gamepad入力（D-Pad & 左スティックY軸）
            if (Gamepad.current != null)
            {
                if (Gamepad.current.dpad.up.wasPressedThisFrame || Gamepad.current.leftStick.ReadValue().y > StickThreshold) upPressed = true;
                if (Gamepad.current.dpad.down.wasPressedThisFrame || Gamepad.current.leftStick.ReadValue().y < -StickThreshold) downPressed = true;
            }

            if (upPressed)
            {
                currentIndex = (currentIndex - 1 + buttons.Length) % buttons.Length;
                SelectButton(currentIndex,playSound:true);
            }
            else if (downPressed)
            {
                currentIndex = (currentIndex + 1) % buttons.Length;
                SelectButton(currentIndex,playSound:true);
            }
        }
    
        /// <summary>
        /// 指定したインデックスのボタンを選択状態にする
        /// </summary>
        /// <param name="index"></param>
        /// <param name="playSound"></param>
        private void SelectButton(int index, bool playSound)
        {
            Sound.Instance.Play(SoundType.ButtonNavi);
            
            // UnityのEventSystemで選択状態を更新（見た目上の強調も含む）
            EventSystem.current.SetSelectedGameObject(buttons[index].gameObject);
        }
    }
}
