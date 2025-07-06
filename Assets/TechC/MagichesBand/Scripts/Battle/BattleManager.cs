using TechC.MagichesBand.Core;
using TechC.MagichesBand.Enemy;
using UnityEngine.UI;
using TechC.MagichesBand.Game;
using UnityEngine;

namespace TechC.MagichesBand.Battle
{
    public class BattleManager : SingletonMonoBehaviour<BattleManager>
    {
        public EnemyFactory factory;
    
        [SerializeField] private RectTransform uiParent; 
        [SerializeField] private GameObject actButton;
        [SerializeField] private GameObject attackCommand;
        [SerializeField] private BattlePlayerController battlePlayerController;
        [SerializeField] private Image effectImage;
        
        public bool playerDead;
        public bool enemyDead;
        
        // 現在の敵を外部から取得可能にする
        public ICharacter CurrentEnemy { get; private set; }

        private void Start()
        {
            effectImage.color = Color.clear;
            
            // UI上に敵を1体生成
            Vector3 spawnPos = new Vector3(0, 40, -23); // anchoredPosition（中央表示）
            var enemy = factory.CreateEnemy(spawnPos, uiParent);

            if (enemy != null)
            {
                CurrentEnemy = enemy;

                var enemyObj = (enemy as MonoBehaviour)?.gameObject;
                if (enemyObj)
                {
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
            effectImage.color = Color.Lerp(effectImage.color, Color.clear, Time.deltaTime);
        }

        public void DamageEffect()
        {
            effectImage.color = new Color(0.4f,0,0,0.4f);
        }
        
        //ButtonControllerに置くと取れなくなるのでここに置く
        public void EnableActButton()
        {
            actButton.SetActive(true);
        }
    
        public void SpawnPhase2Boss()
        {
            Vector3 spawnPos = new Vector3(0, 40, -23); // 表示位置は調整可

            var bossPhase2 = factory.CreateEnemy(spawnPos, uiParent, isPhase2: true);

            if (bossPhase2 != null)
            {
                CurrentEnemy = bossPhase2;

                var enemyObj = (bossPhase2 as MonoBehaviour)?.gameObject;

                if (enemyObj && ButtleTurnManager.Instance)
                {
                    Debug.Log("turnManager");
                    ButtleTurnManager.Instance.ReplaceEnemy(enemyObj);
                    ButtleTurnManager.Instance.SetupTurnOrder();
                }
            }
        }
    
        /// <summary>
        /// playerのpositionを記録
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
