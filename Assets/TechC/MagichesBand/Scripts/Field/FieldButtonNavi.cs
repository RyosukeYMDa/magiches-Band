using UnityEngine;
using TechC.MagichesBand.Core;
using TechC.MagichesBand.Item;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TechC.MagichesBand.Field
{
    /// <summary>
    /// フィールド画面におけるボタンナビゲーションを制御するクラス
    /// </summary>
    public class FieldButtonNavi : MonoBehaviour
    {
        //ボタンを格納
        public Button[] buttons;
        private int currentIndex;
        private const float StickThreshold = 0.5f;
        
        [SerializeField] private InventoryUI inventoryUI;
        
        private void OnEnable()
        {
            currentIndex = 0;
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

            //入力に応じてボタンの選択を上下に移動させる
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
        /// 指定インデックスのボタンを選択状態にし、必要に応じて効果音を再生する
        /// </summary>
        /// <param name="index"></param>
        /// <param name="playSound"></param>
        private void SelectButton(int index, bool playSound)
        {
            if(inventoryUI.isInventory)return;

            if (playSound)
            {
                Sound.Instance.Play(SoundType.ButtonNavi);   
            }
            
            // UnityのEventSystemで選択状態を更新（見た目上の強調も含む）
            EventSystem.current.SetSelectedGameObject(buttons[index].gameObject);
        }

        /// <summary>
        /// 現在の選択位置のボタンを再選択して強調表示を維持する
        /// </summary>
        public void ReSelectButton()
        {
            SelectButton(currentIndex,playSound:false);
        }
    }
}
