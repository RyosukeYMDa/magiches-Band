using System.Collections.Generic;
using TechC.MagichesBand.Core;
using TechC.MagichesBand.Item;
using UnityEngine;

namespace TechC.MagichesBand.Game
{
    /// <summary>
    /// シーンを跨いで使う変数を管理するclass
    /// </summary>
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        [SerializeField] public Vector3 playerPosition;
        [SerializeField] public Quaternion playerRotation;
        [SerializeField] public Quaternion cameraRotation;
        [SerializeField] public Vector3 cameraOffset;
        [SerializeField] private CharacterStatus playerStatus;
    
        //取得したItemのIdのList
        public List<string> obtainedItemIds = new List<string>();
    
        private Vector3 startPlayerPosition;
        private Quaternion startPlayerRotation;
        
        private Vector3 startCameraOffset;
        private Quaternion startCameraRotation;

        public EnemyType enemyType;

        //enemyの発生する場所のtype
        public enum EnemyType
        {
            NotEncounterEnemy,
            Enemy1,
            BossEnemy
        }
    
        //インベントリ
        public Inventory inventory;

        protected override bool dontDestroyOnLoad => true;

        protected override void Awake()
        {
            base.Awake();
            
            startPlayerPosition = new Vector3(-13f, 0.6f, 6);
            startPlayerRotation = Quaternion.Euler(0, 270f, 0);
            
            startCameraOffset = new Vector3(-4f, 3f, 0);
            startCameraRotation = Quaternion.Euler(15f, 90f, 0);
        
            var loadedPlayerData = SaveManager.LoadPlayerData();
            var loadedCameraData = SaveManager.LoadCameraData();
        
            if (loadedPlayerData != null)
            {
                playerPosition = loadedPlayerData.GetPosition();
                Debug.Log("Load Success");
            }
            else
            {
                playerPosition = startPlayerPosition;
                playerRotation = startPlayerRotation;
                Debug.Log("Load Failed");
            }

            if (loadedCameraData != null)
            {
                cameraOffset = loadedCameraData.GetOffset();
                cameraRotation = loadedCameraData.GetRotation();
            }
            else
            {
                cameraOffset = startCameraOffset;
                cameraRotation = startCameraRotation;
            }
        
            // インベントリの読み込み
            var loadedInventory = SaveManager.LoadInventory();
            if (loadedInventory != null)
            {
                inventory = loadedInventory;
                Debug.Log("Inventory Load Success");
            }
            else
            {
                inventory = new Inventory(); // 空で初期化
                Debug.Log("Inventory Load Failed, created new inventory");
            }
        
            //取得済みデータも読み込み
            obtainedItemIds = SaveManager.LoadObtainedItemIds() ?? new List<string>();
        }

        private void ReloadPlayerData()
        {
            var data = SaveManager.LoadPlayerData();
            if (data != null)
            {
                playerPosition = data.GetPosition();
                playerRotation = data.GetRotation();
                Debug.Log("再読み込み成功: " + playerPosition);
            }
            else
            {
                obtainedItemIds = new List<string>();
                playerPosition = startPlayerPosition;
                playerRotation = startPlayerRotation;
                Debug.Log("データなし → 初期位置にリセット: " + playerPosition);
            }
        }

        private void ReloadCameraData()
        {
            var data = SaveManager.LoadCameraData();
            if (data != null)
            {
                cameraOffset = data.GetOffset();
                cameraRotation = data.GetRotation();
            }
            else
            {
                cameraOffset = startCameraOffset;
                cameraRotation = startCameraRotation;
            }
        }
    
        public void DateReset()
        {
            Debug.Log("DateReset");
            playerStatus.hp = playerStatus.maxHp;
            playerStatus.mp = playerStatus.maxMp;
            SaveManager.DeleteAllData();
            ReloadPlayerData();
            ReloadCameraData();
        }
    }
}