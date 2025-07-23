using TechC.MagichesBand.Game;
using UnityEngine;
using UnityEngine.UI;

namespace TechC.MagichesBand.Field
{
    /// <summary>
    /// シェーダーによるディゾルブ演出を制御して画面遷移時のエフェクトを管理するクラス
    /// </summary>
    public class DissolveController : MonoBehaviour
    {
        private static readonly int EffectProgress = Shader.PropertyToID("_EffectProgress");

        [SerializeField] private Image loadingImage;
        [SerializeField] private Material material;
        private float progress; // エフェクト進行度合い
        private bool isPlaying; // エフェクト再生中か
        private Quaternion cameraResidenceAreaRotation; // レジデンスエリアのカメラ回転
        public bool nowLoading; // ローディング中フラグ

        private const float Speed = 1f; // エフェクト速度

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
        /// effectを発生させ、数秒後にstopEffectを実行
        /// </summary>
        /// <returns></returns>
        public void PlayEffect()
        {
            loadingImage.enabled = true;
            isPlaying = true;
            nowLoading = true;
        }

        /// <summary>
        /// effect停止
        /// </summary>
        public void StopEffect()
        {
            isPlaying = false;
            loadingImage.enabled = false;
            nowLoading = false;
        
            Debug.Log("No effect");
        }
    }
}