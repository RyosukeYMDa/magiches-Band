using UnityEngine;

public class Dragon : MonoBehaviour,IEnemy
{
    [SerializeField] private CharacterStatus characterStatus;
    
    public CharacterStatus Status => characterStatus;
    
    public void Attack()
    {
        Debug.Log(characterStatus.def);
    }
}
