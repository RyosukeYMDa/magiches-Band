using UnityEngine;

public class Slime : MonoBehaviour,IEnemy
{
    public void Attack()
    {
        Debug.Log("Slime attacks!");
    }
}
