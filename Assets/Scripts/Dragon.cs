using UnityEngine;

public class Dragon : MonoBehaviour,IEnemy
{
    [SerializeField] private CharacterStatus characterStatus;
    [SerializeField] private BattlePlayerController battlePlayerController;
    public CharacterStatus Status => characterStatus;
    
    public void Attack()
    {
        Debug.Log(characterStatus.def);
        
        if (TurnManager.Instance.CurrentTurnPhase == TurnManager.TurnPhase.FirstMove)
        {
            Debug.Log("Second Move");
            TurnManager.Instance.CurrentTurnPhase = TurnManager.TurnPhase.SecondMove;
            battlePlayerController.Attack();
        }else if (TurnManager.Instance.CurrentTurnPhase == TurnManager.TurnPhase.SecondMove)
        {
            Debug.Log("First Move");
            TurnManager.Instance.CurrentTurnPhase = TurnManager.TurnPhase.FirstMove;
            //TurnManager.Instance.ProceedTurn();
        }
    }
}
