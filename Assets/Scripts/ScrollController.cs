using UnityEngine;

public class ScrollController : MonoBehaviour
{
    public RectTransform rectTransform;
    [SerializeField] private float endPosition;
    
    // Update is called once per frame
    void Update()
    {
        rectTransform.position += new Vector3(0, 0.1f, 0);

        if (rectTransform.anchoredPosition.y > endPosition)
        {
            gameObject.SetActive(false);
        }
    }
}
