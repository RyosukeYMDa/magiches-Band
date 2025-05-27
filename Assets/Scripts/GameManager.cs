using UnityEngine;

/// <summary>
/// シーンを跨いで使う変数を管理するclass
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] public Vector3 playerPosition;

    public int enemyType;
    
    private bool initialized = false;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (initialized == false)
        {
            playerPosition = new Vector3(-13f, 0.6f, 06);
            initialized = true;
        }
    }
}