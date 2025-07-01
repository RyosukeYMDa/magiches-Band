using TechC.MagichesBand.Item;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TechC.MagichesBand
{
    public class ButtonNavigator : MonoBehaviour
    {
    
        //ボタンを格納
        public Button[] buttons;
        private int currentIndex = 0;
        [SerializeField] private InventoryUI inventoryUI;
    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void OnEnable()
        {
            SelectButton(currentIndex);
        }

        // Update is called once per frame
        private void Update()
        {
            if (Keyboard.current.upArrowKey.wasPressedThisFrame && !inventoryUI.isInventory)
            {
                currentIndex = (currentIndex - 1 + buttons.Length) % buttons.Length;
                SelectButton(currentIndex);
            }

            if (!Keyboard.current.downArrowKey.wasPressedThisFrame || inventoryUI.isInventory) return;
        
            currentIndex = (currentIndex + 1) % buttons.Length;
            SelectButton(currentIndex);
        }
    
        private void SelectButton(int index)
        {
            // UnityのEventSystemで選択状態を更新（見た目上の強調も含む）
            EventSystem.current.SetSelectedGameObject(buttons[index].gameObject);
        }

        public void ReSelectButton()
        {
            SelectButton(currentIndex);
        }
    }
}
