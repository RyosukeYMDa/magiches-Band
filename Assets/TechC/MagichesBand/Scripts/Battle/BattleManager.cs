using TechC.MagichesBand.Core;
using TechC.MagichesBand.Enemy;
using TechC.MagichesBand.Field;
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
        
        public bool bossPhase2; //BossのPhase1が撃破されたかの判別
    
        public bool playerDead;
        public bool enemyDead;
        
        // 現在の敵を外部から取得可能にする
        public ICharacter CurrentEnemy { get; private set; }

        private void Start()
        {
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
