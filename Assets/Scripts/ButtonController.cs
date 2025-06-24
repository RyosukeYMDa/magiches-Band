using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class ButtonController : MonoBehaviour
{
//Attackする為のbuttonの参照
    [SerializeField] private GameObject actButton;
    [SerializeField] private GameObject actCommand;
    [SerializeField] private GameObject attackCommand;
    [SerializeField] private ButtonNavigator buttonNavigator;
    [SerializeField] private BattlePlayerController battlePlayerController;
    [SerializeField] private InventoryUI inventoryUI;
    
    //messageを表示させる
    [SerializeField] private TextMeshProUGUI messageText;
    
    [SerializeField] private PlayerInput playerInput;
    
    public GameObject inventoryPanel;
    
    private void OnEnable()
    {
        playerInput.actions["Cancel"].performed += OnAdditionCancel;
    }
    
    private void OnDisable()
    {
        if (!playerInput || !playerInput.actions) return;
        
        // 登録解除
        playerInput.actions["Cancel"].performed -= OnAdditionCancel;
    }

    private void OnAdditionCancel(InputAction.CallbackContext context)
    {
        Debug.Log("OnAdditionCancel");
        attackCommand.SetActive(true);
    }
    
    public void EnableAct()
    {
        if (buttonNavigator.isInventory) return;
        
        Debug.Log("Enabling act");
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
        inventoryUI.OpenInventory();
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

    public void SlashText()
    {
        messageText.gameObject.SetActive(true);
        messageText.text = "slash:enemy - 1";
    }

    public void ExplosionText()
    {
        messageText.gameObject.SetActive(true);
        messageText.text = "Explosion:enemy - 1";
    }

    public void AtkUpText()
    {
        messageText.gameObject.SetActive(true);
        messageText.text = "AtkUp";

        if (battlePlayerController.atkDoublingValue == 16)
        {
            messageText.text = "AtkUpperLimit";
        }
    }

    public void DefUpText()
    {
        messageText.gameObject.SetActive(true);
        messageText.text = "DefUp";

        if (battlePlayerController.defDoublingValue == 16)
        {
            messageText.text = "DefUpperLimit";
        }
    }
}
