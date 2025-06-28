using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public EnemyFactory factory;
    
    [SerializeField] private RectTransform uiParent; 
    [SerializeField] private GameObject actButton;
    [SerializeField] private ButtonNavigator buttonNavigator;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private GameObject attackCommand;
    
    public bool bossPhase2; //BossのPhase1が撃破されたかの判別
    public static BattleManager Instance { get; private set; }
    
    // 現在の敵を外部から取得可能にする
    public ICharacter CurrentEnemy { get; private set; }

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
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
                if (TurnManager.Instance)
                {
                    TurnManager.Instance.AddEnemy(enemyObj);
                    Debug.Log("turnManager");
                    
                    TurnManager.Instance.SetupTurnOrder();
                    
                    TurnManager.Instance.ProceedTurn();
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

            if (enemyObj && TurnManager.Instance)
            {
                TurnManager.Instance.ReplaceEnemy(enemyObj);
                TurnManager.Instance.SetupTurnOrder();
            }
        }
    }
    
    /// <summary>
    /// playerのpositionを記録
    /// </summary>
    public void SavePlayerInventory()
    {
        var playerData = new PlayerData(GameManager.Instance.playerPosition);
        var inventory = GameManager.Instance.inventory;

        SaveManager.SavePlayerData(playerData, inventory);
        Debug.Log("Save"); 
    }
}
