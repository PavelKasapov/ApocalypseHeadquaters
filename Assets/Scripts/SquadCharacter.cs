using UnityEngine;
using Zenject;

public class SquadCharacter : MonoBehaviour, IClickable
{
    [Inject] private readonly SquadControlSystem squadControl;
    [SerializeField] private new Transform transform;
    [SerializeField] float speed = 10f;
    [SerializeField] private SquadUnit squadUnit;
    [SerializeField] private Transform mainSpriteTransform;
    public MovementSystem movementSystem;

    private Vector3 smallerSize = new(0.8f, 0.8f, 1);

    public void MarkSelected(bool isSelected)
    {
        mainSpriteTransform.localScale = isSelected ? smallerSize : Vector3.one;
    }

    public void OnLeftClick(Vector3 position)
    {
        squadControl.SelectCharacter(this, squadUnit);
    }

    public void OnRightClick(Vector3 position){}

}
