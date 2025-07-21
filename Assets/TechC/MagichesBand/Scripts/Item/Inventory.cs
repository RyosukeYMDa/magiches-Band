using System.Collections.Generic;

namespace TechC.MagichesBand.Item
{
    [System.Serializable]
    public class Inventory
    {
        public List<InventoryItem> items = new List<InventoryItem>();

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