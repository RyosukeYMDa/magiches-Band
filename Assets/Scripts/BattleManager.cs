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
    public static BattleManager Instance { get; private set; }
    
    // 現在の敵を外部から取得可能にする
    public IEnemy CurrentEnemy { get; private set; }

    void Awake()
    {
        if (Instance == null)
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
        // UI上にランダムな敵を1体生成
        Vector3 spawnPos = new Vector3(0, 40, -23); // anchoredPosition（中央表示）
        var enemy = factory.CreateEnemy(spawnPos, uiParent);

        if (enemy != null)
        {
            CurrentEnemy = enemy;

            var enemyObj = (enemy as MonoBehaviour)?.gameObject;
            if (enemyObj != null)
            {
                if (TurnManager.Instance != null)
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

    public void EnableActButton()
    {
        actButton.SetActive(true);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame && buttonNavigator.isInventory)
        {
            Debug.Log("closeInventory"); 
            inventoryUI.contentParent.gameObject.SetActive(false);
            buttonNavigator.SetInventoryState(false);
            attackCommand.SetActive(true);
        }
        
        if (Keyboard.current.spaceKey.isPressed)
        {
            SceneManager.LoadScene("MainScene");
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
