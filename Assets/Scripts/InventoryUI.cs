using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject itemTextPrefab; // TextMeshProを含むプレハブ
    [SerializeField] private ButtonNavigator buttonNavigator;
    public Transform contentParent;   // アイテムを並べる親（Vertical Layout Group）
    
    private Inventory inventory;
    
    private int selectedIndex = 0;

    [SerializeField] private CharacterStatus playerStatus;
    
    private List<GameObject> itemUiObjects = new List<GameObject>();

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

        UpdateUI();
    }

    private void Update()
    {
        // 入力バッファ：インベントリ開いた直後の1フレームだけスキップ
        if (buttonNavigator.justOpenedInventory)
        {
            Debug.Log("待機中");
            buttonNavigator.justOpenedInventory = false; // 1フレーム後に解除
            return;
        }
        
        if (!buttonNavigator.isInventory) return;
        
        if (itemUiObjects.Count == 0) return;
        
        if (Keyboard.current.enterKey.wasPressedThisFrame && buttonNavigator.isInventory)
        {
            Debug.Log("enter");
            UseItem(selectedIndex);
        }
        
        if (Keyboard.current.downArrowKey.wasPressedThisFrame && buttonNavigator.isInventory)
        {
            Debug.Log("down");
            selectedIndex = (selectedIndex + 1) % itemUiObjects.Count;
            UpdateHighlight();
        }else if (Keyboard.current.upArrowKey.wasPressedThisFrame && buttonNavigator.isInventory)
        {
            Debug.Log("up");
            selectedIndex = (selectedIndex - 1 + itemUiObjects.Count) % itemUiObjects.Count;
            UpdateHighlight();
        }
    }

    /// <summary>
    /// UIの表示を更新する（全て削除 → 再生成）
    /// </summary>
    private void UpdateUI()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        itemUiObjects.Clear();

        foreach (InventoryItem item in inventory.items)
        {
            GameObject newItem = Instantiate(itemTextPrefab, contentParent);
            //newItem.transform.SetAsLastSibling(); // 新しいのを一番下に
            
            TextMeshProUGUI tmp = newItem.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
                tmp.text = $"{item.itemName} x{item.quantity}";

            itemUiObjects.Add(newItem);
        }

        selectedIndex = 0;
        UpdateHighlight();
    }

    private void UpdateHighlight()
    {
        for (int i = 0; i < itemUiObjects.Count; i++)
        {
            var text = itemUiObjects[i].GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
                text.color = (i == selectedIndex) ? Color.yellow : Color.white;
        }
    }

    private void UseItem(int index)
    {
        if(index < 0 || index >= inventory.items.Count) return;
        
        InventoryItem item = inventory.items[index];
        Debug.Log($"Use Item {item.itemName}");

        if (item.itemName == "Potion")
        {
            inventory.RemoveItem(item.itemName, 1);
            playerStatus.hp = Mathf.Min(playerStatus.hp + 7, playerStatus.maxHp);
            UpdateUI();
            StartCoroutine(MessageReception("Recover 50 HP"));
            isItem = true;
        }else if (item.itemName == "Key")
        {
            if (isOpen)
            {
                Debug.Log("ドアを開けた");
                inventory.RemoveItem(item.itemName, 1);
                GameManager.Instance.enemyType = GameManager.EnemyType.BossEnemy;
                SceneManager.LoadScene("BossScene");
            }
            else
            {
                StartCoroutine(MessageReception("You can't use it here"));
            }
        }
    }

    public IEnumerator MessageReception(string msg)
    {
        messageText.gameObject.SetActive(true);
        messageText.text = msg;
        yield return new WaitForSeconds(1f);
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
        UpdateUI();
    }
}