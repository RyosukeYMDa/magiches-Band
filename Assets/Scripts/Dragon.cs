using UnityEngine;

public class Dragon : MonoBehaviour,IEnemy
{
    public void Attack()
    {
        Debug.Log("Dragon breathes fire!");
    }
}
