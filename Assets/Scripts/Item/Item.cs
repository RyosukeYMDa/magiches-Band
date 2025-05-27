using System;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine;

public class Item : MonoBehaviour
{
    private bool isPlayerInRange = false;
    public GameObject door;
    
    internal string text;

    void Start()
    {
        text = string.Empty;
    }
    
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            isPlayerInRange = true;
            //Debug.Log("プレイヤーがアイテム範囲に入りました");

            door.GetComponent < Door>().Open();
            // 処理例：アイテム回収
            Destroy(this.gameObject);  
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
