using UnityEngine;

namespace TechC.MagichesBand.Core
{
    [System.Serializable]
    public class CameraData
    {
        public float offsetX;
        public float offsetY;
        public float offsetZ;
        
        public float rotX;
        public float rotY;
        public float rotZ;
        public float rotW;

        public CameraData(Vector3 position ,Quaternion rotation)
        {
            this.offsetX = position.x;
            this.offsetY = position.y;
            this.offsetZ = position.z;
            
            this.rotX = rotation.x;
            this.rotY = rotation.y;
            this.rotZ = rotation.z;
            this.rotW = rotation.w;
        }

        public Quaternion GetRotation()
        {
            return new Quaternion(rotX, rotY, rotZ, rotW);
        }
        
        public Vector3 GetOffset()
        {
            return new Vector3(offsetX, offsetY, offsetZ);
        }
    }
}