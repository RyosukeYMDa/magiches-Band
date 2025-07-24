using System.Collections.Generic;
using System;

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

        // 変更が発生したときに通知するイベント
        public event Action OnInventoryChanged;
        
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
            OnInventoryChanged?.Invoke();
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
            OnInventoryChanged?.Invoke();
        }
        
        /// <summary>
        /// 所持しているかどうかを簡単にチェックする補助関数
        /// </summary>
        public bool HasItem(string itemName, int minAmount = 1)
        {
            var item = items.Find(i => i.itemName == itemName);
            return item != null && item.quantity >= minAmount;
        }

        /// <summary>
        /// 所持数を返す ない場合は0
        /// </summary>
        public int GetQuantity(string itemName)
        {
            var item = items.Find(i => i.itemName == itemName);
            return item?.quantity ?? 0;
        }
    }
}