using TechC.MagichesBand.Core;
using TechC.MagichesBand.Enemy;
using UnityEngine.UI;
using TechC.MagichesBand.Game;
using UnityEngine;

namespace TechC.MagichesBand.Battle
{
    /// <summary>
    /// バトル全体の管理を行うクラス
    /// 敵の生成 UI制御 ダメージ演出などを担当する
    /// </summary>
    public class BattleManager : SingletonMonoBehaviour<BattleManager>
    {
        public EnemyFactory factory; // 敵キャラクターを生成するファクトリクラスの参照
        
        [SerializeField] private RectTransform uiParent; 
        [SerializeField] private GameObject actButton;
        [SerializeField] private GameObject attackCommand;
        [SerializeField] private BattlePlayerController battlePlayerController;
        [SerializeField] private Image effectImage;
        
        public bool playerDead; // プレイヤーが倒れたかどうかを管理するフラグ
        public bool enemyDead; // 敵が倒れたかどうかを管理するフラグ
        
        // 現在の敵を外部から取得可能にする
        public ICharacter CurrentEnemy { get; private set; }

        //キャッシュ
        private GameManager gameManager;

        protected override void Awake()
        {
            base.Awake();
            gameManager = GameManager.Instance;
        }
        
        private void Start()
        {
            effectImage.color = Color.clear;
            
            // UI上に敵を1体生成
            var spawnPos = new Vector3(0, 40, -23); // anchoredPosition（中央表示）
            var enemy = factory.CreateEnemy(spawnPos, uiParent); // 敵を生成して変数に保持する

            if (enemy != null)
            {
                CurrentEnemy = enemy; // 現在の敵として記録する

                var enemyObj = (enemy as MonoBehaviour)?.gameObject;
                if (enemyObj)
                {
                    //ターン初期化処理を行う
                    if (ButtleTurnManager.Instance)
                    {
                        ButtleTurnManager.Instance.AddEnemy(enemyObj);
                        Debug.Log("turnManager");
                    
                        ButtleTurnManager.Instance.SetupTurnOrder();
                    
                        ButtleTurnManager.Instance.ProceedTurn();
                    }
                    else
                    {
                        Debug.LogError("[BattleManager] TurnManager.Instance が null です。シーンに配置されているか確認してください。");
                    }
                }
            }

            Debug.Log("[BattleManager] Enemy created and passed to TurnManager.");
        }

        private void Update()
        {
            // ダメージやバフ演出用のイメージの色を徐々に透明に近づける
            effectImage.color = Color.Lerp(effectImage.color, Color.clear, Time.deltaTime);
        }

        /// <summary>
        /// ダメージ演出処理 敵やプレイヤーがダメージを受けたときに画面を赤くする
        /// </summary>
        public void DamageEffect()
        {
            effectImage.color = new Color(0.4f,0,0,0.4f);
        }

        /// <summary>
        /// バフ演出処理 ステータス上昇時に画面を緑っぽく光らせる
        /// </summary>
        public void BuffEffect()
        {
            effectImage.color = new Color(0,0.7f,0.5f,0.9f);
        }
        
        /// <summary>
        /// 行動ボタンの表示処理
        /// ButtonControllerに置くと参照できないためこちらに配置している
        /// </summary>
        public void EnableActButton()
        {
            actButton.SetActive(true);
        }
    
        /// <summary>
        /// フェーズ2のボスを出現させる処理
        /// 第二形態などの切り替えタイミングで使用する
        /// </summary>
        public void SpawnPhase2Boss()
        {
            var spawnPos = new Vector3(0, 40, -23); // 表示位置は調整可

            // Phase2ボスとして生成するフラグをtrueにして生成する
            var bossPhase2 = factory.CreateEnemy(spawnPos, uiParent, isPhase2: true);

            if (bossPhase2 == null) return;
            CurrentEnemy = bossPhase2;

            var enemyObj = (bossPhase2 as MonoBehaviour)?.gameObject;

            if (!enemyObj || !ButtleTurnManager.Instance) return;
            
            Debug.Log("turnManager");
            ButtleTurnManager.Instance.ReplaceEnemy(enemyObj); // 新しい敵でターン順を再構成してターン進行する
        }
    
        /// <summary>
        /// プレイヤーの現在位置 回転 カメラ情報とインベントリをセーブファイルに記録する
        /// </summary>
        public void SavePlayerInventory()
        {
            var playerData = new PlayerData(GameManager.Instance.playerPosition, GameManager.Instance.playerRotation);
            var cameraDate = new CameraData(GameManager.Instance.cameraOffset,GameManager.Instance.cameraRotation);
            var inventory = GameManager.Instance.inventory;

            SaveManager.SavePlayerData(playerData, inventory,cameraDate);
            Debug.Log("Save"); 
        }
    }
}
