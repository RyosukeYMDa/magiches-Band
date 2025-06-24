using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

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
        if (!buttonNavigator.justOpenedInventory) return;
        
        Debug.Log("待機中");
        buttonNavigator.justOpenedInventory = false; // 1フレーム後に解除
    }
    
    public void OpenInventory()
    {   
        UpdateUI(); // 忘れずに最新のUIを生成

        if (itemUiObjects.Count <= 0) return;
        
        EventSystem.current.SetSelectedGameObject(itemUiObjects[0]);
        selectedIndex = 0;
        UpdateHighlight();
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        Debug.Log("OnNavigate called!");
        
        if (!buttonNavigator.isInventory || itemUiObjects.Count <= 0) return;

        Vector2 input = context.ReadValue<Vector2>();

        if (input.y < -0.5f)
        {
            // 下に移動
            selectedIndex = (selectedIndex + 1) % itemUiObjects.Count;
            UpdateHighlight();
            Debug.Log("Navigate Down");
        }
        else if (input.y > 0.5f)
        {
            // 上に移動
            selectedIndex = (selectedIndex - 1 + itemUiObjects.Count) % itemUiObjects.Count;
            UpdateHighlight();
            Debug.Log("Navigate Up");
        }
    }
    
    public void OnSubmit(InputAction.CallbackContext context)
    {
        Debug.Log("OnNavigate called!");
        
        if (!buttonNavigator.isInventory || itemUiObjects.Count <= 0) return;

        UseItem(selectedIndex);
        Debug.Log("Submit");
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        Debug.Log("closeInventory"); 
        if(!buttonNavigator.isInventory) return;
        
        isItem = false;
        contentParent.gameObject.SetActive(false);
        buttonNavigator.SetInventoryState(false);
        
        
        Debug.Log("closeInventory");
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

        switch (item.itemName)
        {
            case "Potion":
                inventory.RemoveItem(item.itemName, 1);
                playerStatus.hp = Mathf.Min(playerStatus.hp + 7, playerStatus.maxHp);
                UpdateUI();
                StartCoroutine(MessageReception("Recover 7 HP"));
                isItem = true;
                break;
            case "Key" when isOpen:
                Debug.Log("ドアを開けた");
                inventory.RemoveItem(item.itemName, 1);
                GameManager.Instance.enemyType = GameManager.EnemyType.BossEnemy;
                SceneManager.LoadScene("BossScene");
                break;
            case "Key":
                StartCoroutine(MessageReception("You can't use it here"));
                break;
            case "MpPotion":
                inventory.RemoveItem(item.itemName, 1);
                playerStatus.mp = Mathf.Min(playerStatus.mp + 7, playerStatus.maxMp);
                UpdateUI();
                StartCoroutine(MessageReception("Recover 7 MP"));
                isItem = true;
                break;
        }
    }

    public void GetItem(string itemName)
    {
        StartCoroutine(MessageReception(itemName + ":get"));
    }
    
    //waitが機能してない
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
        UpdateUI();
    }
}