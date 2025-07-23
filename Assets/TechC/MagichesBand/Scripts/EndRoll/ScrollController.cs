using TechC.MagichesBand.Core;
using UnityEngine;

namespace TechC.MagichesBand.EndRoll
{
    /// <summary>
    /// エンドロールのスクロール演出を制御するためのクラス
    /// RectTransform を徐々に上へ移動させて規定位置に到達したら感謝テキストを表示する
    /// </summary>
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
