using UnityEngine;

namespace TechC.MagichesBand.Core
{
    /// <summary>
    /// プレイヤーの位置と回転に関する情報を保存するためのクラス
    /// SaveManagerを通じてセーブデータとして保存や復元を行う用途で使用される
    /// </summary>
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
        
        /// <summary>
        /// playerのpositionとrotation
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
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
        
        /// <summary>
        /// 保存されている各位置座標を使ってVector3型の位置情報を再構築して返す
        /// </summary>
        /// <returns></returns>
        public Vector3 GetPosition()
        {
            return new Vector3(posX, posY, posZ);
        }
        
        /// <summary>
        /// 保存されている各回転成分を使ってQuaternion型の回転情報を再構築して返す
        /// </summary>
        /// <returns></returns>
        public Quaternion GetRotation()
        {
            return new Quaternion(rotX, rotY, rotZ, rotW);
        }
    }
}
