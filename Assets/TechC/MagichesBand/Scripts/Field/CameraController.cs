using TechC.MagichesBand.Game;
using UnityEngine;

namespace TechC.MagichesBand.Field
{
    /// <summary>
    /// カメラの位置・回転を制御するクラス
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform target;
        private Vector3 offset; // ターゲットからの相対位置オフセット

        private void Start()
        {
            // カメラ位置 = ターゲット位置 + offset
            offset = GameManager.Instance.cameraOffset;
            transform.position = target.position + offset;
            transform.rotation = GameManager.Instance.cameraRotation;
        }

        /// <summary>
        /// LateUpdateでターゲット追従
        /// </summary>
        private void LateUpdate()
        {
            transform.position = target.position + offset;
        }

        /// <summary>
        /// カメラを指定角度だけ回転させ、GameManagerに回転・位置情報を保存
        /// </summary>
        /// <param name="angle">回転角度（Y軸周り）</param>
        public void CamRotation(float angle)
        {
            Debug.Log("CameraRotate");

            transform.RotateAround(target.position, Vector3.up, angle);

            offset = transform.position - target.position;
            GameManager.Instance.cameraRotation = transform.rotation;
            GameManager.Instance.cameraOffset = offset;
        }
    }
}