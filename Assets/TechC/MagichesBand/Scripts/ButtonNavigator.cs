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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        SelectButton(currentIndex);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Keyboard.current.upArrowKey.wasPressedThisFrame && !isInventory)
        {
            currentIndex = (currentIndex - 1 + buttons.Length) % buttons.Length;
            SelectButton(currentIndex);
        }

        if (!Keyboard.current.downArrowKey.wasPressedThisFrame || isInventory) return;
        
        currentIndex = (currentIndex + 1) % buttons.Length;
        SelectButton(currentIndex);
    }

    public void SetInventoryState(bool state)
    {
        isInventory = state;
        Debug.Log($"SetInventoryState: {isInventory}");
    }
    
    void SelectButton(int index)
    {
        // UnityのEventSystemで選択状態を更新（見た目上の強調も含む）
        EventSystem.current.SetSelectedGameObject(buttons[index].gameObject);
    }
}
