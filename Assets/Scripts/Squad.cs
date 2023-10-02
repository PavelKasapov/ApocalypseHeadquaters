using UnityEngine;

public class Squad : MonoBehaviour
{
    [SerializeField] float squadWideness = 1f;

    [SerializeField] private Character[] characters;

    public void MoveSquad(Vector3 pointToMove)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            Character character = characters[i];

            if (!character.gameObject.activeSelf) continue;

            character.MovementSystem.MoveCharacter(pointToMove);
        }
    }

    public void ChaseAndAttack(ITargetInfo targetInfo)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            Character character = characters[i];

            if (!character.gameObject.activeSelf) continue;

            character.CombatSystem.ChaseAndAttack(targetInfo);
        }
    }
}
