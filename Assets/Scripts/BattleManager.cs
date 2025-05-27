using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public EnemyFactory factory;
    public TurnManager turnManager;
    
    [SerializeField] private RectTransform uiParent; 
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        // UI上にランダムな敵を1体生成
        Vector3 spawnPos = new Vector3(-300, -87, -23); // anchoredPosition（中央表示）
        var enemy = factory.CreateEnemy(spawnPos, uiParent);
        
        if (enemy != null)
        {
            // 作成した敵の GameObject を TurnManager に渡す
            var enemyObj = (enemy as MonoBehaviour)?.gameObject;
            if (enemyObj != null)
            {
                turnManager.AddEnemy(enemyObj);
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
