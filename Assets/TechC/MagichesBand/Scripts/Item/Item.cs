using TechC.MagichesBand.Core;
using TechC.MagichesBand.Game;
using UnityEngine;

namespace TechC.MagichesBand.Item
{
    /// <summary>
    /// フィールド上に存在する取得可能なアイテムを制御するクラス
    /// </summary>
    [System.Serializable]
    public class Item : MonoBehaviour
    {
        [Header("アイテム情報")]
        [SerializeField] public string itemId; // 識別用ID
        [SerializeField] public string itemName;   // item名
        [SerializeField] public int amount = 1;    // 取得できる個数

        [SerializeField] private InventoryUI inventoryUI;
    
        private void Start()
        {
            // 取得済みなら削除
            if (GameManager.Instance.obtainedItemIds.Contains(itemId))
            {
                Destroy(gameObject);
            }
        }
    
        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.gameObject.CompareTag("Player")) return;
            
            inventoryUI.GetItem(itemName);
            inventoryUI.SetInventoryItem(itemId, itemName, amount);
            GameManager.Instance.obtainedItemIds.Add(itemId);
            SaveManager.SaveObtainedItemIds(GameManager.Instance.obtainedItemIds);
            Destroy(this.gameObject); // アイテムを消す
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("プレイヤーがアイテム範囲から出ました");
            }
        }
    }
}