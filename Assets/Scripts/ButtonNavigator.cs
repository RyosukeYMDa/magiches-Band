using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ButtonNavigator : MonoBehaviour
{
    
    //ボタンを格納
    public Button[] buttons;
    private int currentIndex = 0;
    
    //Inventoryが表示されているのかを判別
    public bool isInventory;
    
    public bool justOpenedInventory;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        SelectButton(currentIndex);
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame && !isInventory)
        {
            buttons[currentIndex].onClick.Invoke(); // 選択中のボタンを押す
        }
        
        if (Keyboard.current.upArrowKey.wasPressedThisFrame && !isInventory)
        {
            currentIndex = (currentIndex - 1 + buttons.Length) % buttons.Length;
            SelectButton(currentIndex);
        }

        if (Keyboard.current.downArrowKey.wasPressedThisFrame && !isInventory)
        {
            currentIndex = (currentIndex + 1) % buttons.Length;
            SelectButton(currentIndex);
        }
    }

    public void IsInventorySwitch()
    {
        Debug.Log("IsInventorySwitch");
        if (isInventory)
        {
            isInventory = false;
        }
        else
        {
            isInventory = true;
        }
    }
    
    void SelectButton(int index)
    {
        // UnityのEventSystemで選択状態を更新（見た目上の強調も含む）
        EventSystem.current.SetSelectedGameObject(buttons[index].gameObject);
    }
}
