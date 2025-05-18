using UnityEngine;


/// <summary>
/// シーンを跨いで使う変数を管理するclass
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance{get; private set;}
    
    public Transform playerPosition;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
