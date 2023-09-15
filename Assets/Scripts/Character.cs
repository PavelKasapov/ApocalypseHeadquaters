using System.Collections;
using UnityEngine;
using Zenject;

public class Character : MonoBehaviour, IClickable, ITarget
{
    [SerializeField] private Squad squadUnit;
    [SerializeField] private GameObject selectedCircle;
    [SerializeField] private EntityType entityType;

    [Inject(Id = "MainTransform")] private Transform selfTransform;
    [Inject] private SightSystem sightSystem;
    [Inject] private MovementSystem movementSystem;
    [Inject] private HpSystem hpSystem;
    //[Inject] private RangeAttack rangeAttack;

    public MovementSystem MovementSystem => movementSystem;
    public EntityType EntityType => entityType;
    public Transform Transform => selfTransform;
    public Squad Squad => squadUnit;
    public HpSystem HpSystem => hpSystem;

    private void Awake()
    {
        sightSystem.Initialize(entityType, 90, 15);
    }

    public void MarkSelected(bool isSelected)
    {
        selectedCircle.SetActive(isSelected);
    }
}


