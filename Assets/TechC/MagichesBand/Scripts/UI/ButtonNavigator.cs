using TechC.MagichesBand.Core;
using TechC.MagichesBand.Item;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TechC.MagichesBand.UI
{
    public class ButtonNavigator : MonoBehaviour
    {
    
        //ボタンを格納
        public Button[] buttons;
        private int currentIndex = 0;
        [SerializeField] private InventoryUI inventoryUI;
        
        private void OnEnable()
        {
            currentIndex = 0;
            SelectButton(currentIndex);
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
                SelectButton(currentIndex);
            }
            else if (downPressed)
            {
                currentIndex = (currentIndex + 1) % buttons.Length;
                SelectButton(currentIndex);
            }
        }
    
        private void SelectButton(int index)
        {
            if(inventoryUI.isInventory)return;
            
            Sound.Instance.Play(SoundType.ButtonNavi);
            
            // UnityのEventSystemで選択状態を更新（見た目上の強調も含む）
            EventSystem.current.SetSelectedGameObject(buttons[index].gameObject);
        }
    }
}
