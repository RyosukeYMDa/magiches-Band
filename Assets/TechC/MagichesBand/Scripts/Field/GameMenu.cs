using System.Collections;
using TechC.MagichesBand.Item;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace TechC.MagichesBand.Field
{
    /// <summary>
    ///     セーブ等のメニューを管理するクラス
    /// </summary>
    public class GameMenu : MonoBehaviour
    {
        [Header("パネルの設定")]
        public RectTransform panel;          // スライドさせるパネル
        public Vector2 shownPosition;        // 表示時の位置
        public Vector2 hiddenPosition;       // 非表示時の位置
        public float slideDuration = 0.3f;   // スライド時間
    
        private Coroutine slideCoroutine;

        [SerializeField] private ButtonNavigator buttonNavigator;
        [SerializeField] private InventoryUI inventoryUI;
        [FormerlySerializedAs("characterController")] [SerializeField] private PlayerController playerController;
        [SerializeField] private PlayerInput playerInput;
        public GameObject inventoryPanel;

        private void OnEnable()
        {
            var menuAction = playerInput.actions["Menu"];

            menuAction.performed += Menu;
        }

        private void OnDisable()
        {
            if (playerInput && playerInput.actions)
            {
                var menuAction = playerInput.actions["Menu"];

                menuAction.performed -= Menu;   
            }
        }
    
        public void Menu(InputAction.CallbackContext context)
        {
            Debug.Log("Menu");
            if(buttonNavigator.isInventory) return;
        
            TogglePanel();
        }

        private void TogglePanel()
        {
            // すでにスライド中なら中断
            if (slideCoroutine != null)
                StopCoroutine(slideCoroutine);

            slideCoroutine = StartCoroutine(SlidePanel(playerController.isShown ? hiddenPosition : shownPosition)); 
            playerController.isShown = !playerController.isShown;
        
        }

        IEnumerator SlidePanel(Vector2 targetPosition)
        {
            Vector2 startPos = panel.anchoredPosition;
            float elapsed = 0f;
            bool hiding = targetPosition == hiddenPosition;

            while (elapsed < slideDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / slideDuration);
                panel.anchoredPosition = Vector2.Lerp(startPos, targetPosition, t);
                yield return null;
            }

            panel.anchoredPosition = targetPosition;
            slideCoroutine = null;
        
            if (hiding && buttonNavigator.isInventory)
            {
                gameObject.SetActive(false);
            }
        }

        public void InventoryDisplay()
        {
            Debug.Log("InventoryDisplay"); 
            TogglePanel();
            inventoryPanel.SetActive(true);
            buttonNavigator.SetInventoryState(true);
        }
    }
}