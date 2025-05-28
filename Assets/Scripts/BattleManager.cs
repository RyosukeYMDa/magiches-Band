using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public EnemyFactory factory;
    
    [SerializeField] private RectTransform uiParent; 
    
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
        Vector3 spawnPos = new Vector3(-300, -87, -23); // anchoredPosition（中央表示）
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

    // Update is called once per frame
    private void Update()
    {
        if (Keyboard.current.spaceKey.isPressed)
        {
            SceneManager.LoadScene("MainScene");
        } 
    }
}
