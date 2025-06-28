using UnityEngine;

namespace TechC.MagichesBand.Field
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private GameObject targetObject;
        private Vector3 offset;

        [SerializeField] private Transform target;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            offset = gameObject.transform.position - targetObject.transform.position;
        }

        // Update is called once per frame
        private void LateUpdate()
        {
            gameObject.transform.position = targetObject.transform.position + offset;
        }

        public void CamRotation(float angle)
        {
            var targetPosition = target.position;

            Debug.Log("CameraRotate");
            transform.RotateAround(targetPosition, Vector3.up, angle);

            offset = transform.position - targetPosition;
        }
    }
}