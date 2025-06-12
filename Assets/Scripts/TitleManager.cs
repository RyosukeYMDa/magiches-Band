using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private CharacterStatus playerStatus;
    
    private void Start()
    {
        PlayerData data = SaveManager.LoadPlayerData();
        if (data != null)
        {
            GameManager.Instance.playerPosition = data.GetPosition();
            Debug.Log("Load");
        }
    }
    
    public void StartGame()
    {
        Debug.Log("StartGame");
        SceneManager.LoadScene("MainScene");
    }
    
    public void DateReset()
    {
        Debug.Log("DateReset");
        playerStatus.hp = playerStatus.maxHp;
        playerStatus.mp = playerStatus.maxMp;
        SaveManager.DeleteAllData();
        GameManager.Instance.ReloadPlayerData();
    }
}
