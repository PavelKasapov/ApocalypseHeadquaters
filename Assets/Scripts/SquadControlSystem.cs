using UnityEngine;

public class SquadControlSystem : MonoBehaviour
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
            selectedSquad.MoveSquad(clickedPoint);
        } else if (selectedCharacter != null)
        {
            selectedCharacter.MovementSystem.MoveCharacter(clickedPoint);
        }
    }
}
