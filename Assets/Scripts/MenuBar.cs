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

    private bool isShown = false;        // 現在表示状態か
    private Coroutine slideCoroutine;

    void Update()
    {
        // Escキーで切り替え
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePanel();
        }
    }

    public void TogglePanel()
    {
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

        while (elapsed < slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / slideDuration);
            panel.anchoredPosition = Vector2.Lerp(startPos, targetPosition, t);
            yield return null;
        }

        panel.anchoredPosition = targetPosition;
        slideCoroutine = null;
    }
}
