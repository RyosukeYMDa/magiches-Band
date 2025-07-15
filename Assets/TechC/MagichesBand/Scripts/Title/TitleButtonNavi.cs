using UnityEngine;
using TechC.MagichesBand.Core;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TechC.MagichesBand.Title
{
    public class TitleButtonNavi : MonoBehaviour
    {
    
        //ボタンを格納
        public Button[] buttons;
        private int currentIndex = 0;
        
        private void Start()
        {
            currentIndex = 0;
            SelectTitleButton(currentIndex);
        }

        private void Update()
        {
            bool upPressed = false;
            bool downPressed = false;

            // Gamepad入力（D-Pad & 左スティックY軸）
            if (Gamepad.current != null)
            {
                if (Gamepad.current.dpad.up.wasPressedThisFrame || Gamepad.current.leftStick.ReadValue().y > 0.5f) upPressed = true;
                if (Gamepad.current.dpad.down.wasPressedThisFrame || Gamepad.current.leftStick.ReadValue().y < -0.5f) downPressed = true;
            }

            if (upPressed)
            {
                currentIndex = (currentIndex - 1 + buttons.Length) % buttons.Length;
                SelectTitleButton(currentIndex);
            }
            else if (downPressed)
            {
                currentIndex = (currentIndex + 1) % buttons.Length;
                SelectTitleButton(currentIndex);
            }
        }
    
        private void SelectTitleButton(int index)
        {
            Sound.Instance.Play(SoundType.ButtonSelect);
            
            // UnityのEventSystemで選択状態を更新（見た目上の強調も含む）
            EventSystem.current.SetSelectedGameObject(buttons[index].gameObject);
        }
    }
}
