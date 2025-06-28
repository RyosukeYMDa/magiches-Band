using UnityEngine;

namespace TechC.MagichesBand
{
    [System.Serializable]
    public class PlayerData
    {
        public float posX;
        public float posY;
        public float posZ;

        public PlayerData(Vector3 position)
        {
            this.posX = position.x;
            this.posY = position.y;
            this.posZ = position.z;
        }
    
        public Vector3 GetPosition()
        {
            return new Vector3(posX, posY, posZ);
        }
    }
}
