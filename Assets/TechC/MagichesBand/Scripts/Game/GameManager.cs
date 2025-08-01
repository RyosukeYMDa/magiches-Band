using System.Collections;
using System.Collections.Generic;
using TechC.MagichesBand.Core;
using TechC.MagichesBand.Item;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

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

        public EnemyType enemyType; // 現在の敵タイプ

        //enemyの発生する場所のtype
        public enum EnemyType
        {
            NotEncounterEnemy,
            Enemy1,
            BossEnemy
        }
    
        //インベントリ
        public Inventory inventory;

        // シーン切り替えで破棄しない
        protected override bool dontDestroyOnLoad => true;

        /// <summary>
        /// 初期化：セーブデータからプレイヤー・カメラ情報を読み込む
        /// </summary>
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
            
            
        }

        /// <summary>
        /// スタート時にインベントリと取得済みアイテムIDを読み込み
        /// </summary>
        private void Start()
        {
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
        
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        /// Sceneを跨いだ際に実行される
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            StartCoroutine(DelayedApply());
        }

        /// <summary>
        /// UI が初期化されるまで待つ関数
        /// </summary>
        /// <returns></returns>
        private IEnumerator DelayedApply()
        {
            yield return null; // UI が初期化されるまで待つ
            ApplyMouseAndUISettings();
        }

        /// <summary>
        /// マウスを無効化させる
        /// </summary>
        private void ApplyMouseAndUISettings()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            // マウスだけ無効化する（キーボードやゲームパッドは無効化しない）
            if (Mouse.current != null && Mouse.current.enabled)
                InputSystem.DisableDevice(Mouse.current);
        }

        /// <summary>
        /// プレイヤーデータを再読み込み（セーブデータから）
        /// </summary>
        private void ReloadPlayerData()
        {
            var data = SaveManager.LoadPlayerData();
            if (data != null)
            {
                playerPosition = data.GetPosition();
                playerRotation = data.GetRotation();
                Debug.Log($"再読み込み成功: {playerPosition}");
            }
            else
            {
                obtainedItemIds = new List<string>();
                playerPosition = startPlayerPosition;
                playerRotation = startPlayerRotation;
                Debug.Log($"データなし → 初期位置にリセット: {playerPosition}");
            }
        }

        /// <summary>
        /// カメラデータを再読み込み（セーブデータから）
        /// </summary>
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
    
        /// <summary>
        /// ゲームデータを初期化（HP/MP回復、インベントリリセットなど）
        /// </summary>
        public void DateReset()
        {
            Debug.Log("DateReset");
            playerStatus.hp = playerStatus.maxHp;
            playerStatus.mp = playerStatus.maxMp;
            SaveManager.DeleteAllData();
            obtainedItemIds = new List<string>();
            inventory = new Inventory();
            ReloadPlayerData();
            ReloadCameraData();
        }
    }
}