namespace TechC.MagichesBand.Item
{
    /// <summary>
    /// インベントリ内の1つのアイテム情報を表すクラス
    /// </summary>
    [System.Serializable]
    public class InventoryItem
    {
        public string itemId;   //個体識別（ユニークID）
        public string itemName; //種類（表示名）
        public int quantity;    //個数

        public InventoryItem(string id, string itemName, int quantity)
        {
            itemId = id;
            this.itemName = itemName;
            this.quantity = quantity;
        }
    }
}