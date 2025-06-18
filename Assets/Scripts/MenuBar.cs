using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class MenuBar : MonoBehaviour
{
    [Header("パネルの設定")]
    public RectTransform panel;          // スライドさせるパネル
    public Vector2 shownPosition;        // 表示時の位置
    public Vector2 hiddenPosition;       // 非表示時の位置
    public float slideDuration = 0.3f;   // スライド時間

    public bool isShown;        // 現在表示状態か
    private Coroutine slideCoroutine;

    [SerializeField] private ButtonNavigator buttonNavigator;
    public GameObject inventoryPanel;
    private void Update()
    {
        // Escキーで切り替え
        if (Keyboard.current.escapeKey.wasPressedThisFrame && !buttonNavigator.isInventory)
        {
            TogglePanel();
        }
    }

    public void TogglePanel()
    {
        // 表示状態にする前にこのオブジェクトを有効化（SetActive(true)）
        if (!isShown)
            panel.gameObject.SetActive(true);
        
        // すでにスライド中なら中断
        if (slideCoroutine != null)
            StopCoroutine(slideCoroutine);

        slideCoroutine = StartCoroutine(SlidePanel(isShown ? hiddenPosition : shownPosition));
        isShown = !isShown;
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
        
        // 完全に非表示位置になったら MenuBar を無効化
        if (hiding)
        {
            panel.gameObject.SetActive(false);
        }
    }

    public void InventoryDisplay()
    {
        if(buttonNavigator.justOpenedInventory)return;
        Debug.Log("InventoryDisplay");
        StartCoroutine(ShowInventoryPanelNextFrame());
    }

    IEnumerator ShowInventoryPanelNextFrame()
    {
        // バッファ先にセット
        buttonNavigator.justOpenedInventory = true;

        yield return null;

        inventoryPanel.SetActive(true);
        panel.gameObject.SetActive(false);
        buttonNavigator.SetInventoryState(true);
    }
}