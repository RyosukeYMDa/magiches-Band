using System.Collections;
using TechC.MagichesBand.Core;
using TechC.MagichesBand.Item;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace TechC.MagichesBand.Field
{
    /// <summary>
    /// フィールド中のセーブやインベントリ表示などメニュー操作全般を管理するクラス
    /// </summary>
    public class GameMenu : MonoBehaviour
    {
        [Header("パネルの設定")]
        [SerializeField] public RectTransform panel;      // スライドさせるパネル
        [SerializeField] public Vector2 shownPosition;    // 表示時の位置
        [SerializeField] public Vector2 hiddenPosition;   // 非表示時の位置
        private const float SlideDuration = 0.3f;         // スライド時間

        private Coroutine slideCoroutine;

        [SerializeField] private FieldButtonNavi fieldButtonNavi;
        [SerializeField] private InventoryUI inventoryUI;
        [SerializeField] private DissolveController dissolveController;
        [FormerlySerializedAs("characterController")] [SerializeField] private PlayerController playerController;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] public GameObject inventoryPanel;

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

        /// <summary>
        /// // メニューを閉じる処理
        /// </summary>
        public void CloseMenu()
        {
            // 二重起動防止
            if (slideCoroutine != null)
                StopCoroutine(slideCoroutine);

            slideCoroutine = StartCoroutine(SlidePanel(hiddenPosition));
            playerController.isShown = false;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// // パネルを一定時間かけて指定位置へスライドさせるコルーチン処理
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <returns></returns>
        private IEnumerator SlidePanel(Vector2 targetPosition)
        {
            Sound.Instance.Play(SoundType.MenuSlide);
            
            var startPos = panel.anchoredPosition;
            var elapsed = 0f;
            var hiding = targetPosition == hiddenPosition;

            while (elapsed < SlideDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / SlideDuration);
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

        /// <summary>
        /// // インベントリ画面を開く処理
        /// </summary>
        public void InventoryDisplay()
        {
            Debug.Log("InventoryDisplay");
            Sound.Instance.Play(SoundType.ButtonSelect);
            CloseMenu();
            inventoryPanel.SetActive(true);
            inventoryUI.SetInventoryState(true);
        }
        
        /// <summary>
        /// タイトル画面に遷移する
        /// </summary>
        public void ReturnTitle()
        {
            playerController.SavePlayerPosition();
            SceneManager.LoadScene("Title");
        }
    }
}