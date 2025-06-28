using System.Collections.Generic;
using TechC.MagichesBand.Game;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace TechC.MagichesBand.Item
{
    public class ItemSelect : MonoBehaviour
    {
        private readonly List<GameObject> itemUiObjects = new();
    
        private int selectedIndex = 0;
    
        [SerializeField] private ButtonNavigator buttonNavigator;
    
        public Transform contentParent;   // アイテムを並べる親（Vertical Layout Group）
    
        [SerializeField] private CharacterStatus playerStatus;
    
        [SerializeField] private InventoryUI inventoryUI;
    
        [SerializeField] private GameObject itemTextPrefab; // TextMeshProを含むプレハブ
    
        [SerializeField] private PlayerInput playerInput;
    
        private void OnEnable()
        {
            // PlayerInputが設定されていなければ警告だけ出す
            if (playerInput == null)
            {
                Debug.LogWarning("PlayerInputが設定されていません！");
                return;
            }
        
            OpenInventory();
        
            var navigateAction = playerInput.actions["Choice"];
            var submitAction = playerInput.actions["UiSubmit"];
            var cancelAction = playerInput.actions["UiCancel"];

            // イベント登録
            navigateAction.performed += OnNavigate;
            submitAction.performed += OnSubmit;
            cancelAction.performed += OnCancel;
        }

        private void OnDisable()
        {
            if (playerInput && playerInput.actions)
            {
                var navigateAction = playerInput.actions["Choice"];
                var submitAction = playerInput.actions["UiSubmit"];
                var cancelAction = playerInput.actions["UiCancel"];

                // イベント登録
                navigateAction.performed -= OnNavigate;
                submitAction.performed -= OnSubmit;
                cancelAction.performed -= OnCancel;   
            }
        }
    
        private void OpenInventory()
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
        
            inventoryUI.isItem = false;
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

            foreach (InventoryItem item in inventoryUI.inventory.items)
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
            if(index < 0 || index >= inventoryUI.inventory.items.Count) return;
        
            InventoryItem item = inventoryUI.inventory.items[index];
            Debug.Log($"Use Item {item.itemName}");

            switch (item.itemName)
            {
                case "Potion":
                    inventoryUI.inventory.RemoveItem(item.itemName, 1);
                    playerStatus.hp = Mathf.Min(playerStatus.hp + 7, playerStatus.maxHp);
                    UpdateUI();
                    StartCoroutine(inventoryUI.MessageReception("Recover 7 HP"));
                    inventoryUI.isItem = true;
                    break;
                case "Key" when inventoryUI.isOpen:
                    Debug.Log("ドアを開けた");
                    inventoryUI.inventory.RemoveItem(item.itemName, 1);
                    GameManager.Instance.enemyType = GameManager.EnemyType.BossEnemy;
                    SceneManager.LoadScene("BossScene");
                    break;
                case "Key":
                    StartCoroutine(inventoryUI.MessageReception("You can't use it here"));
                    break;
                case "MpPotion":
                    inventoryUI.inventory.RemoveItem(item.itemName, 1);
                    playerStatus.mp = Mathf.Min(playerStatus.mp + 7, playerStatus.maxMp);
                    UpdateUI();
                    StartCoroutine(inventoryUI.MessageReception("Recover 7 MP"));
                    inventoryUI.isItem = true;
                    break;
            }
        }
    }
}
