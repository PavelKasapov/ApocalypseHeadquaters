using UnityEngine;
using Zenject;

public class SquadUnit : MonoBehaviour
{
    [SerializeField] float squadWideness = 1f;

    [SerializeField] private SquadCharacter[] characters;

    public void MoveSquad(Vector3 pointToMove)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            SquadCharacter character = characters[i];
            character.movementSystem.MoveCharacter(pointToMove);
        }
    }
}
