using System.Collections;
using TechC.MagichesBand.Core;
using TechC.MagichesBand.Item;
using TechC.MagichesBand.UI;
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

        [SerializeField] private FieldButtonNavi fieldButtonNavi;
        [SerializeField] private InventoryUI inventoryUI;
        [SerializeField] private DissolveController dissolveController;
        [FormerlySerializedAs("characterController")] [SerializeField] private PlayerController playerController;
        [SerializeField] private PlayerInput playerInput;
        public GameObject inventoryPanel;

        private void OnEnable()
        {
            if (playerController.isShown && inventoryUI.isInventory || dissolveController.nowLoading)
            {
                gameObject.SetActive(false);
                return;
            }
            
            fieldButtonNavi.ReSelectButton();
            OpenMenu();
        }
        
        private void OpenMenu()
        {
            // 二重起動防止
            if (slideCoroutine != null)
                StopCoroutine(slideCoroutine);
            
            slideCoroutine = StartCoroutine(SlidePanel(shownPosition));
            playerController.isShown = true;
        }

        public void CloseMenu()
        {
            // 二重起動防止
            if (slideCoroutine != null)
                StopCoroutine(slideCoroutine);

            slideCoroutine = StartCoroutine(SlidePanel(hiddenPosition));
            playerController.isShown = false;
            gameObject.SetActive(false);
        }

        private IEnumerator SlidePanel(Vector2 targetPosition)
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
        
            if (hiding)
            {
                gameObject.SetActive(false);
            }
        }

        public void InventoryDisplay()
        {
            Debug.Log("InventoryDisplay");
            Sound.Instance.Play(SoundType.ButtonSelect);
            CloseMenu();
            inventoryPanel.SetActive(true);
            inventoryUI.SetInventoryState(true);
        }
    }
}