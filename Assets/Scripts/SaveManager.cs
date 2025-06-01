using UnityEngine;

public static class SaveManager
{
    private const string PlayerKey = "PlayerData";

    public static void SavePlayerData(PlayerData playerData)
    {
        string json = JsonUtility.ToJson(playerData);
        PlayerPrefs.SetString(PlayerKey, json);
        PlayerPrefs.Save();
        Debug.Log("保存完了" + json);
    }

    public static PlayerData LoadPlayerData()
    {
        if(!PlayerPrefs.HasKey(PlayerKey))return null;
        
        string json = PlayerPrefs.GetString(PlayerKey);
        PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);
        return playerData;
    }
    
    public static void DeletePlayerData()
    {
        PlayerPrefs.DeleteKey(PlayerKey);
        Debug.Log("保存データを削除");
    }
}
