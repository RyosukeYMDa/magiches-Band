using UnityEngine;

namespace TechC.MagichesBand.Core
{
    [System.Serializable]
    public class PlayerData
    {
        // 位置
        public float posX;
        public float posY;
        public float posZ;

        // 回転
        public float rotX;
        public float rotY;
        public float rotZ;
        public float rotW;
        
        public PlayerData(Vector3 position, Quaternion rotation)
        {
            this.posX = position.x;
            this.posY = position.y;
            this.posZ = position.z;
            
            this.rotX = rotation.x;
            this.rotY = rotation.y;
            this.rotZ = rotation.z;
            this.rotW = rotation.w;
        }
    
        public Vector3 GetPosition()
        {
            return new Vector3(posX, posY, posZ);
        }
        
        public Quaternion GetRotation()
        {
            return new Quaternion(rotX, rotY, rotZ, rotW);
        }
    }
}
