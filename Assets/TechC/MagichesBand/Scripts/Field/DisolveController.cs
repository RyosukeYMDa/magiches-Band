using System.Collections;
using TechC.MagichesBand.Game;
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
        private Quaternion cameraResidenceAreaRotation;
        public bool nowLoading;

        private const float Speed = 1f;

        //ループの周期
        private const float LoopDuration = 1f;

        public enum AreaState
        {
            ResidenceArea,
            RuinsArea
        }

        public AreaState areaState = AreaState.ResidenceArea;

        private void Awake()
        {
            cameraResidenceAreaRotation = Quaternion.Euler(15f, 90f, 0);
        }
        
        private void Start()
        {
            if (GameManager.Instance.cameraRotation != cameraResidenceAreaRotation)
            {
                areaState = AreaState.RuinsArea;
            }
        }
        
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
        public void PlayEffect()
        {
            loadingImage.enabled = true;
            isPlaying = true;
            nowLoading = true;
        }

        public void StopEffect()
        {
            isPlaying = false;
            loadingImage.enabled = false;
            nowLoading = false;
        
            Debug.Log("No effect");
        }
    }
}