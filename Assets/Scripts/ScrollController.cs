using UnityEngine;

public class ScrollController : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private float endPosition;
    [SerializeField] private GameObject buttons;
    
    // Update is called once per frame
    private void Update()
    {
        rectTransform.position += new Vector3(0, 0.1f, 0);

        if (rectTransform.anchoredPosition.y > endPosition)
        {
            buttons.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
