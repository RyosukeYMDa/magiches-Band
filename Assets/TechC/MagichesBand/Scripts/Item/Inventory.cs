using System.Collections.Generic;

namespace TechC.MagichesBand.Item
{
    /// <summary>
    /// 複数のアイテムを管理するインベントリクラス
    /// </summary>
    [System.Serializable]
    public class Inventory
    {
        // 所持アイテム一覧
        public List<InventoryItem> items = new List<InventoryItem>();

        /// <summary>
        /// 指定したアイテムを追加
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemName"></param>
        /// <param name="amount"></param>
        public void AddItem(string id, string itemName, int amount)
        {
            var item = items.Find(i => i.itemName == itemName);
            if (item != null)
            {
                item.quantity += amount;
            }
            else
            {
                items.Add(new InventoryItem(id, itemName, amount));
            }
        }

        /// <summary>
        /// 指定したアイテムを削除
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="amount"></param>
        public void RemoveItem(string itemName, int amount)
        {
            var item = items.Find(i => i.itemName == itemName);

            if (item == null) return;
            
            item.quantity -= amount;
            if (item.quantity <= 0)
            {
                items.Remove(item);
            }
        }
    }
}