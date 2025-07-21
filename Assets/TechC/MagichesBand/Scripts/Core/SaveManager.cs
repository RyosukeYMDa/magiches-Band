using System.Collections.Generic;
using TechC.MagichesBand.Item;
using UnityEngine;

namespace TechC.MagichesBand.Core
{
    public static class SaveManager
    {
        private const string PlayerKey = "PlayerData";
        private const string CameraKey = "CameraData";

        private const string InventoryKey = "InventoryData";
    
        //使用してもIdを記憶させる
        private const string ObtainedItemsKey = "ObtainedItems";
    
        public static void SavePlayerData(PlayerData playerData, Inventory inventory, CameraData cameraData)
        {
            var playerJson = JsonUtility.ToJson(playerData);
            var cameraJson = JsonUtility.ToJson(cameraData);
            var inventoryJson = JsonUtility.ToJson(inventory);
        
            PlayerPrefs.SetString(PlayerKey, playerJson);
            PlayerPrefs.SetString(CameraKey, cameraJson);
            PlayerPrefs.SetString(InventoryKey, inventoryJson);
            Debug.Log($"Inventory Save JSON: {inventoryJson}");
            PlayerPrefs.Save();
            Debug.Log($"保存完了:{playerJson} / {inventoryJson} / {cameraJson}");
        }
        
        public static void SaveInventory(Inventory inventory)
        {
            var json = JsonUtility.ToJson(inventory);
            PlayerPrefs.SetString(InventoryKey, json);
            PlayerPrefs.Save();
            Debug.Log($"Inventory saved: {json}");
        }
    
        public static void SaveObtainedItemIds(List<string> itemIds)
        {
            var wrapper = new StringListWrapper(itemIds);
            var json = JsonUtility.ToJson(wrapper);
            PlayerPrefs.SetString(ObtainedItemsKey, json);
            PlayerPrefs.Save();
            Debug.Log($"取得済みアイテムIDを保存: {json}");
        }
    
        public static List<string> LoadObtainedItemIds()
        {
            if (!PlayerPrefs.HasKey(ObtainedItemsKey))
            {
                Debug.Log("取得済みアイテムデータが存在しません");
                return new List<string>();
            }

            var json = PlayerPrefs.GetString(ObtainedItemsKey);
            var wrapper = JsonUtility.FromJson<StringListWrapper>(json);
            Debug.Log($"取得済みアイテムロード: {json}");
            return wrapper.idList;
        }

        public static Inventory LoadInventory()
        {
            if (!PlayerPrefs.HasKey(InventoryKey))
            {
                Debug.Log("インベントリデータが存在しません");
                return null;
            }

            var json = PlayerPrefs.GetString(InventoryKey);
            Debug.Log($"Inventory Load JSON: {json}");
        
            var inventory = JsonUtility.FromJson<Inventory>(json);
            return inventory;
        }
    
        public static PlayerData LoadPlayerData()
        {
            if(!PlayerPrefs.HasKey(PlayerKey))return null;
        
            var json = PlayerPrefs.GetString(PlayerKey);
            var playerData = JsonUtility.FromJson<PlayerData>(json);
            return playerData;
        }

        public static CameraData LoadCameraData()
        {
            if(!PlayerPrefs.HasKey(CameraKey))return null;

            var json = PlayerPrefs.GetString(CameraKey);
            var cameraDate = JsonUtility.FromJson<CameraData>(json);
            return cameraDate;
        }
    
        public static void DeleteAllData()
        {
            PlayerPrefs.DeleteKey(InventoryKey);
            PlayerPrefs.DeleteKey(PlayerKey);
            PlayerPrefs.DeleteKey(CameraKey);
            PlayerPrefs.DeleteKey(ObtainedItemsKey);
            PlayerPrefs.Save();
            Debug.Log("保存データを削除");
        }
    }
}
