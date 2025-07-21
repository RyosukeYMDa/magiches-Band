using TechC.MagichesBand.Core;
using UnityEngine;

namespace TechC.MagichesBand.EndRoll
{
    public class ScrollController : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private float endPosition;
        [SerializeField] private GameObject thankText;

        private void Start()
        {
            Sound.Instance.Play(SoundType.EndingRollBGM,true);
        }
        
        // Update is called once per frame
        private void Update()
        {
            rectTransform.position += new Vector3(0, 0.1f, 0);

            if (!(rectTransform.anchoredPosition.y > endPosition)) return;
            
            gameObject.SetActive(false);
            thankText.SetActive(true);
        }
    }
}
