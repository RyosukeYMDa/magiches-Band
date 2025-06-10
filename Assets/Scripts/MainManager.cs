using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class MainManager : MonoBehaviour
{
    [SerializeField] private GameObject enemy1;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private ButtonNavigator buttonNavigator;
    
    public Vector3 player;


    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance が null です！シーンに GameManager が存在しているか確認してください。");
            return;
        }

        if (GameManager.Instance.enemyType == 0)
        {
            enemy1.SetActive(true);
        }

        if (GameManager.Instance.enemyType == 1)
        {
            StartCoroutine(EnableSpawnPoint());
        }
    }

    private void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame && buttonNavigator.isInventory)
        {
            Debug.Log("closeInventory"); 
            inventoryUI.contentParent.gameObject.SetActive(false);
            buttonNavigator.SetInventoryState(false);
        }
    }

    private IEnumerator EnableSpawnPoint()
    {
        yield return new WaitForSeconds(5f);
        enemy1.SetActive(true);
    }
}
