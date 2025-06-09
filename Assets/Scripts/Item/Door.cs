using Unity.VisualScripting;
using UnityEngine;

public class Door : MonoBehaviour
{
    
    [SerializeField] InventoryUI inventoryUI;
    private void OnCollisionEnter(Collision collision)
    {
        inventoryUI.isOpen = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        inventoryUI.isOpen = false;
    }
}
