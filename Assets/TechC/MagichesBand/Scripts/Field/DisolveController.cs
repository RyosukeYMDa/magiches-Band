using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TechC.MagichesBand.Field
{
    /// <summary>
    ///     画面遷移のコントローラー
    /// </summary>
    public class DissolveController : MonoBehaviour
    {
        private static readonly int EffectProgress = Shader.PropertyToID("_EffectProgress");

        [SerializeField] private Image loadingImage;
        [SerializeField] private Material material;
        private float progress;
        private bool isPlaying;

        private const float Speed = 1f;

        //ループの周期
        private const float LoopDuration = 1f;

        public enum AreaState
        {
            ResidenceArea,
            RuinsArea
        }

        public AreaState areaState = AreaState.ResidenceArea;

        // Update is called once per frame
        private void Update()
        {
            if (!isPlaying) return;

            progress += Time.deltaTime * Speed;

            var loopedProgress = Mathf.Repeat(progress, LoopDuration);

            var normalized = loopedProgress / LoopDuration;

            material.SetFloat(EffectProgress, normalized);
        }

        /// <summary>
        ///     effectを発生させ、数秒後にstopEffectを実行
        /// </summary>
        /// <returns></returns>
        public IEnumerator PlayEffect()
        {
            loadingImage.enabled = true;
            isPlaying = true;


            yield return new WaitForSeconds(2.0f);

            StopEffect();
        }

        private void StopEffect()
        {
            isPlaying = false;
            loadingImage.enabled = false;
        
            Debug.Log("No effect");
        }

        public void ResetEffect()
        {
            progress = 0f;
            material.SetFloat(EffectProgress, 0f);
        }
    }
}