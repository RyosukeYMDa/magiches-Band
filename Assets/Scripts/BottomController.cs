using UnityEngine;

public class BottomController : MonoBehaviour
{
    //Attackする為のbuttonの参照
    [SerializeField] private GameObject actButton;
    [SerializeField] private GameObject attackButton;
    [SerializeField] private GameObject attackCommand;
    
    public void EnableAct()
    {
        attackButton.SetActive(true);
        actButton.SetActive(false);
    }

    public void EnableAttack()
    {
        attackCommand.SetActive(true);
        attackButton.SetActive(false);
    }
}
