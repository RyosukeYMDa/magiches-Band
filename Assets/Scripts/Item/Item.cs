using UnityEngine;

[System.Serializable]
public class Item : MonoBehaviour
{
    [Header("アイテム情報")]
    public string itemId;
    public string itemName;   // item名
    public int amount = 1;    // 取得できる個数

    private bool isPlayerInRange = false;

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
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = true;
            inventoryUI.SetInventoryItem(itemId, itemName, 1);
            GameManager.Instance.obtainedItemIds.Add(itemId);
            SaveManager.SaveObtainedItemIds(GameManager.Instance.obtainedItemIds);
            StartCoroutine(inventoryUI.MessageReception(itemName + ":get"));
            Destroy(this.gameObject); // アイテムを消す
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("プレイヤーがアイテム範囲から出ました");
        }
    }
}