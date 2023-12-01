using UnityEngine;

public class ActiveSquadControl : MonoBehaviour
{
    private Character selectedCharacter;
    private Squad selectedSquad;

    public void SelectCharacter(SquadMember clickedCharacter)
    {
        selectedCharacter = clickedCharacter;
        
        if (selectedCharacter != null) 
        {
            selectedSquad = clickedCharacter.Squad;
        }
        
    }

    public void MoveToPoint(Vector2 clickedPoint)
    {
        if (selectedSquad != null) 
        {
            selectedSquad.ChaseAndAttack(null);
            selectedSquad.MoveSquad(clickedPoint);
        }   
        else if (selectedCharacter != null)
        {
            selectedCharacter.CombatSystem.HardLockTarget(null);
            selectedCharacter.MovementSystem.MoveToPoint(clickedPoint);
        }
    }

    public void ChaseAndAttack(ITargetInfo targetInfo)
    {
        if (selectedSquad != null)
        {
            selectedSquad.ChaseAndAttack(targetInfo);
            selectedSquad.MoveSquad(targetInfo.Transform.position);
        }
        else if (selectedCharacter != null)
        {
            selectedCharacter.CombatSystem.HardLockTarget(targetInfo);
            selectedCharacter.MovementSystem.MoveToPoint(targetInfo.Transform.position);
        }
    }
}