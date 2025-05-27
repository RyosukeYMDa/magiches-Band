using UnityEngine;

public class Door : MonoBehaviour
{
    
    public GameObject door;

    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = door.GetComponent<Rigidbody>();
    }

    public void Open()
    {
        rb.isKinematic = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
