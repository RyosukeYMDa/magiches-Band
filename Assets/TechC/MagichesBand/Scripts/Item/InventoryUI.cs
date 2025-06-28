using UnityEngine;
using TMPro;
using System.Collections;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private ButtonNavigator buttonNavigator;
    
    public Inventory inventory;

    public bool isOpen; //扉の範囲内にいるかどうか
    public bool isItem; //item使ったかどうか 
    
    //messageを表示させる
    [SerializeField] private TextMeshProUGUI messageText;

    private void Start()
    {
        // Inventoryを初期化 or 既存のものを取得
        inventory = SaveManager.LoadInventory();
        if (inventory == null)
        {
            inventory = new Inventory();
        }
    }
    
    public void GetItem(string itemName)
    {
        StartCoroutine(MessageReception(itemName + ":get"));
    }
    
    public IEnumerator MessageReception(string msg)
    {
        Debug.Log("Message reception");
        messageText.gameObject.SetActive(true);
        messageText.text = msg;
        Debug.Log("到達前");
        yield return new WaitForSeconds(1.0f);
        Debug.Log("到達");
        messageText.gameObject.SetActive(false);
    }

    /// <summary>
    /// インベントリの外部アクセス用
    /// </summary>
    public void SetInventoryItem(string itemId, string itemName, int quantity)
    {
        Debug.Log($"SetInventoryItem {itemName}");
        inventory.AddItem(itemId, itemName, quantity);
        GameManager.Instance.inventory.AddItem(itemId, itemName, quantity);
        //UpdateUI();
    }
}