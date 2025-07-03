using TechC.MagichesBand.Game;
using UnityEngine;

namespace TechC.MagichesBand.Field
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform target;
        private Vector3 offset;

        private void Start()
        {
            // カメラ位置 = ターゲット位置 + offset
            offset = GameManager.Instance.cameraOffset;
            transform.position = target.position + offset;
            transform.rotation = GameManager.Instance.cameraRotation;
        }

        private void LateUpdate()
        {
            transform.position = target.position + offset;
        }

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