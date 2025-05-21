using UnityEngine;

public class Dragon : MonoBehaviour,IEnemy
{
    [SerializeField] private CharacterStatus characterStatus;
    
    public void Attack()
    
    {
        Debug.Log(characterStatus.def);
    }
}
