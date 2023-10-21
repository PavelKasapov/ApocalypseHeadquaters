using UnityEngine;

public class ActiveSquadControl : MonoBehaviour
{
    private Character selectedCharacter;
    private Squad selectedSquad;

    public void SelectCharacter(Character clickedCharacter)
    {
        selectedCharacter?.MarkSelected(false);
        selectedCharacter = clickedCharacter;
        
        if (selectedCharacter != null) 
        {
            selectedSquad = clickedCharacter.Squad;
            selectedCharacter.MarkSelected(true);
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
            selectedCharacter.CombatSystem.ChaseAndAttack(null);
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
            selectedCharacter.CombatSystem.ChaseAndAttack(targetInfo);
            selectedCharacter.MovementSystem.MoveToPoint(targetInfo.Transform.position);
        }
    }
}