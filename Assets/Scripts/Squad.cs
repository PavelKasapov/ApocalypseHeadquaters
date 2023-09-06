using UnityEngine;
using Zenject;

public class Squad : MonoBehaviour
{
    [SerializeField] float squadWideness = 1f;

    [SerializeField] private Character[] characters;

    public void MoveSquad(Vector3 pointToMove)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            Character character = characters[i];
            character.MovementSystem.MoveCharacter(pointToMove);
        }
    }
}
