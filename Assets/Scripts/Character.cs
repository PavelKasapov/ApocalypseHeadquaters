using UnityEngine;
using Zenject;

public class Character : MonoBehaviour, IClickable, ITargetInfo
{
    [SerializeField] private Squad squadUnit;
    [SerializeField] private GameObject selectedCircle;
    [SerializeField] private EntityType entityType;

    [Inject(Id = "MainTransform")] private Transform selfTransform;
    [Inject] private SightSystem sightSystem;
    [Inject] private MovementSystem movementSystem;
    [Inject] private CombatSystem combatSystem;

    public MovementSystem MovementSystem => movementSystem;
    public EntityType EntityType => entityType;
    public Transform Transform => selfTransform;
    public Squad Squad => squadUnit;
    public CombatSystem CombatSystem => combatSystem;

    private void Awake()
    {
        sightSystem.Initialize(90, 15);
    }

    public void MarkSelected(bool isSelected)
    {
        selectedCircle.SetActive(isSelected);
    }
}


