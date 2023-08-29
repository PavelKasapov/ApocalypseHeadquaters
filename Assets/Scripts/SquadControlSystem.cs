using UnityEngine;

public class SquadControlSystem : MonoBehaviour
{
    private SquadCharacter selectedCharacter;
    private SquadUnit selectedSquad;

    public void SelectCharacter(SquadCharacter clickedCharacter, SquadUnit charactersSquad)
    {
        selectedCharacter?.MarkSelected(false);
        selectedCharacter = clickedCharacter;
        selectedSquad = charactersSquad;
        selectedCharacter.MarkSelected(true);
    }

    public void MoveToPoint(Vector2 clickedPoint)
    {
        if (selectedSquad != null) 
        {
            selectedSquad.MoveSquad(clickedPoint);
        } else if (selectedCharacter != null)
        {
            selectedCharacter.movementSystem.MoveCharacter(clickedPoint);
        }
    }

    public void UnselectCharacter()
    {
        selectedCharacter?.MarkSelected(false);
        selectedCharacter = null;
        selectedSquad = null;
    }
}
