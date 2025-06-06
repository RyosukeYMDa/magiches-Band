using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// シーンを跨いで使う変数を管理するclass
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] public Vector3 playerPosition;
    
    //取得したItemのIdのList
    public List<string> obtainedItemIds = new List<string>();
    
    private Vector3 startPosition;

    //enemyの発生する場所のtype
    public int enemyType;
    
    //インベントリ
    public Inventory inventory;

    private void Awake()
    {
        startPosition = new Vector3(-13f, 0.6f, 6);
        
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        var loadedData = SaveManager.LoadPlayerData();
        
        if (loadedData != null)
        {
            playerPosition = loadedData.GetPosition();
            Debug.Log("Load Success");
        }
        else
        {
            playerPosition = startPosition;
            Debug.Log("Load Failed");
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
    
    public void ReloadPlayerData()
    {
        var data = SaveManager.LoadPlayerData();
        if (data != null)
        {
            playerPosition = data.GetPosition();
            Debug.Log("再読み込み成功: " + playerPosition);
        }
        else
        {
            obtainedItemIds = new List<string>();
            playerPosition = startPosition;
            Debug.Log("データなし → 初期位置にリセット: " + playerPosition);
        }
    }
}