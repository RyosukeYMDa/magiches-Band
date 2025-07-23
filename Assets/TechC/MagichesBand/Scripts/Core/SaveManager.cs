using System.Collections.Generic;
using TechC.MagichesBand.Item;
using UnityEngine;

namespace TechC.MagichesBand.Core
{
    /// <summary>
    /// プレイヤーやインベントリなどの各種データをPlayerPrefsを用いて保存および読み込みするための静的クラス
    /// </summary>
    public static class SaveManager
    {
        private const string PlayerKey = "PlayerData"; // PlayerData保存用キー
        private const string CameraKey = "CameraData"; // CameraData保存用キー
        private const string InventoryKey = "InventoryData"; // Inventory保存用キー
    
        //使用してもIdを記憶させる
        private const string ObtainedItemsKey = "ObtainedItems";
    
        /// <summary>
        /// プレイヤーデータとインベントリおよびカメラ情報をまとめて保存する
        /// </summary>
        public static void SavePlayerData(PlayerData playerData, Inventory inventory, CameraData cameraData)
        {
            // 各データをJSON文字列に変換
            var playerJson = JsonUtility.ToJson(playerData);
            var cameraJson = JsonUtility.ToJson(cameraData);
            var inventoryJson = JsonUtility.ToJson(inventory);
        
            // PlayerPrefsに保存
            PlayerPrefs.SetString(PlayerKey, playerJson);
            PlayerPrefs.SetString(CameraKey, cameraJson);
            PlayerPrefs.SetString(InventoryKey, inventoryJson);
            Debug.Log($"Inventory Save JSON: {inventoryJson}");
            PlayerPrefs.Save(); // 保存
            Debug.Log($"保存完了:{playerJson} / {inventoryJson} / {cameraJson}");
        }
        
        /// <summary>
        /// インベントリデータのみを保存する
        /// </summary>
        public static void SaveInventory(Inventory inventory)
        {
            var json = JsonUtility.ToJson(inventory);
            PlayerPrefs.SetString(InventoryKey, json);
            PlayerPrefs.Save();
            Debug.Log($"Inventory saved: {json}");
        }
    
        /// <summary>
        /// プレイヤーが取得済みのアイテムIDリストを保存する
        /// </summary>
        public static void SaveObtainedItemIds(List<string> itemIds)
        {
            // JSONUtilityで保存できる形式にするためラッパーを使う
            var wrapper = new StringListWrapper(itemIds);
            var json = JsonUtility.ToJson(wrapper);
            
            PlayerPrefs.SetString(ObtainedItemsKey, json);
            PlayerPrefs.Save();
            Debug.Log($"取得済みアイテムIDを保存: {json}");
        }
    
        /// <summary>
        /// 取得済みアイテムIDリストを読み込む
        /// データが存在しない場合は空のリストを返す
        /// </summary>
        public static List<string> LoadObtainedItemIds()
        {
            // データが保存されていない場合は空のリストを返す
            if (!PlayerPrefs.HasKey(ObtainedItemsKey))
            {
                Debug.Log("取得済みアイテムデータが存在しません");
                return new List<string>();
            }

            // JSONを取得してデシリアライズ
            var json = PlayerPrefs.GetString(ObtainedItemsKey);
            var wrapper = JsonUtility.FromJson<StringListWrapper>(json);
            Debug.Log($"取得済みアイテムロード: {json}");
            return wrapper.idList;
        }

        /// <summary>
        /// 保存されたインベントリデータを読み込む
        /// データが存在しない場合はnullを返す
        /// </summary>
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
    
        /// <summary>
        /// 保存されたプレイヤーデータを読み込む
        /// データが存在しない場合はnullを返す
        /// </summary>
        public static PlayerData LoadPlayerData()
        {
            if(!PlayerPrefs.HasKey(PlayerKey))return null;
        
            var json = PlayerPrefs.GetString(PlayerKey);
            var playerData = JsonUtility.FromJson<PlayerData>(json);
            return playerData;
        }

        /// <summary>
        /// 保存されたカメラデータを読み込む
        /// データが存在しない場合はnullを返す
        /// </summary>
        public static CameraData LoadCameraData()
        {
            if(!PlayerPrefs.HasKey(CameraKey))return null;

            var json = PlayerPrefs.GetString(CameraKey);
            var cameraDate = JsonUtility.FromJson<CameraData>(json);
            return cameraDate;
        }
    
        /// <summary>
        /// 全ての保存データを削除する
        /// プレイヤー インベントリ カメラ 取得済みアイテムの全てが対象
        /// </summary>
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
