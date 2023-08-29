using UnityEngine;
using Zenject;

public class GroundClickDetector : MonoBehaviour, IClickable
{
    [Inject] private SquadControlSystem squadControlSystem;
    public void OnLeftClick(Vector3 position)
    {
        squadControlSystem.UnselectCharacter();
    }

    public void OnRightClick(Vector3 position)
    {
        squadControlSystem.MoveToPoint(position);
    }
}
