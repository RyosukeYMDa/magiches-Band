using UnityEngine;

namespace TechC.MagichesBand.Core
{
    /// <summary>
    /// カメラの位置オフセットと回転情報を保存および復元するためのデータクラス
    /// セーブやロードなどに使用できるよう [System.Serializable] 属性が付けられている
    /// </summary>
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

        /// <summary>
        /// CameraのpositionとRotation
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
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

        /// <summary>
        /// 保存されたfloat値からQuaternionを復元して返す
        /// </summary>
        /// <returns></returns>
        public Quaternion GetRotation()
        {
            return new Quaternion(rotX, rotY, rotZ, rotW);
        }
        
        /// <summary>
        /// 保存されたfloat値からVector3を復元して返す
        /// </summary>
        /// <returns></returns>
        public Vector3 GetOffset()
        {
            return new Vector3(offsetX, offsetY, offsetZ);
        }
    }
}