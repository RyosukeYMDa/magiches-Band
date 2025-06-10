using UnityEngine;
using System.Collections;

public class BottomController : MonoBehaviour
{
    //Attackする為のbuttonの参照
    [SerializeField] private GameObject actButton;
    [SerializeField] private GameObject actCommand;
    [SerializeField] private GameObject attackCommand;
    [SerializeField] private ButtonNavigator buttonNavigator;
    
    public GameObject inventoryPanel;
    public void EnableAct()
    {
        if (buttonNavigator.isInventory) return;
        
        actCommand.SetActive(true);
        actButton.SetActive(false);
    }

    public void EnableAttack()
    {
        if (buttonNavigator.isInventory) return;
        
        attackCommand.SetActive(true);
        actCommand.SetActive(false);
    }
    
    public void InventoryDisplay()
    {
        Debug.Log($"InventoryDisplay called at frame {Time.frameCount}");
        if (buttonNavigator.isInventory) return;
        
        if(buttonNavigator.justOpenedInventory)return;
        Debug.Log("InventoryDisplay");
        StartCoroutine(ShowInventoryPanelNextFrame());
    }

    /// <summary>
    /// buttonの受け付けが重複しないようにコルーチンを使う
    /// </summary>
    /// <returns></returns>
    IEnumerator ShowInventoryPanelNextFrame()
    {
        // バッファ先にセット
        buttonNavigator.justOpenedInventory = true;

        yield return null;

        inventoryPanel.SetActive(true);
        actCommand.SetActive(false);
        buttonNavigator.SetInventoryState(true);
    }
}
