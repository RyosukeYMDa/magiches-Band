using System;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine;

public class Item : MonoBehaviour
{
    private bool isPlayerInRange = false;
    
    internal string text;

    void Start()
    {
        text = string.Empty;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("プレイヤーがアイテム範囲に入りました");
            // 処理例：アイテム回収
            Destroy(gameObject);  
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("プレイヤーがアイテム範囲から出ました");
        }
    }
}
