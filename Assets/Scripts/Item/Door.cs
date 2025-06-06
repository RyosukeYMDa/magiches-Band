using Unity.VisualScripting;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isOpen;

    private void OnCollisionEnter(Collision collision)
    {
        isOpen = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isOpen = false;
    }
}
