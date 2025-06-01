using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// シーンを跨いで使う変数を管理するclass
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] public Vector3 playerPosition;
    
    private Vector3 startPosition;

    //enemyの発生する場所のtype
    public int enemyType;

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
        
        PlayerData loadedData = SaveManager.LoadPlayerData();
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
    }
    
    private void Update()
    {
        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            PlayerData data = SaveManager.LoadPlayerData();
            if (data != null)
            {
                playerPosition = data.GetPosition();
                Debug.Log("Load");
            }
        }

        if (Keyboard.current.bKey.wasPressedThisFrame)
            SaveManager.DeletePlayerData();
    }
}