using UnityEngine;
using Zenject;

public abstract class Character : MonoBehaviour, IClickable, ITargetInfo
{
    [Inject(Id = "MainTransform")] private Transform selfTransform;
    [Inject] private SightSystem sightSystem;
    [Inject] private MovementSystem movementSystem;
    [Inject] private CombatSystem combatSystem;
    [Inject] private SelectorHandler selectorHandler;

    public MovementSystem MovementSystem => movementSystem;
    public abstract EntityType EntityType { get; }
    public Transform Transform => selfTransform;


    public CombatSystem CombatSystem => combatSystem;
    public SelectorHandler SelectorHandler => selectorHandler;

    private void Awake()
    {
        sightSystem.Initialize(90, 15, EntityType);
    }
}