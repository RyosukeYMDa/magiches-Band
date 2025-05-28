using UnityEngine;


public class BattlePlayerController : MonoBehaviour,IEnemy
{
    [SerializeField] private CharacterStatus characterStatus;
    
    public void Attack()
    {
        Debug.Log(characterStatus.def);
        
        IEnemy enemy = BattleManager.Instance.CurrentEnemy;
        
        if (TurnManager.Instance.CurrentTurnPhase == TurnManager.TurnPhase.FirstMove)
        {
            Debug.Log("Second Move");
            TurnManager.Instance.CurrentTurnPhase = TurnManager.TurnPhase.SecondMove;
            enemy.Attack();
        }else if(TurnManager.Instance.CurrentTurnPhase == TurnManager.TurnPhase.SecondMove)
        {
            Debug.Log("First Move");
            TurnManager.Instance.CurrentTurnPhase = TurnManager.TurnPhase.FirstMove;
            //TurnManager.Instance.ProceedTurn();
        }
    }
    
    public CharacterStatus Status => characterStatus;
}
