using System.Collections;
using UnityEngine;
using Zenject;

public class Character : MonoBehaviour, IClickable, ITarget, ICoroutineHandler
{
    //[Inject] private readonly SquadControlSystem squadControl;
    [SerializeField] private Transform selfTransform;
    [SerializeField] private Transform modelHolderTransform;
    [SerializeField] private Squad squadUnit;
    [SerializeField] private GameObject selectedCircle;
    [SerializeField] private SightSystem sightSystem;
    [SerializeField] private MovementSystem movementSystem;
    [SerializeField] private EntityType entityType;

    private HpSystem hpSystem;

    public MovementSystem MovementSystem => movementSystem;
    public EntityType EntityType => entityType;
    public Transform Transform => selfTransform;
    public Transform ModelTransform => modelHolderTransform;
    public Squad Squad => squadUnit;
    public HpSystem HpSystem => hpSystem;
    public SightSystem SightSystem => sightSystem;

    private void Awake()
    {
        hpSystem = new HpSystem(selfTransform, 10);
        sightSystem.Initialize(entityType, 90, 15);
        //movementSystem = new MovementSystem(this, SightSystem, ModelTransform, Transform, 3f, 240f);
    }

    public void MarkSelected(bool isSelected)
    {
        //mainSpriteTransform.localScale = isSelected ? smallerSize : Vector3.one;
        selectedCircle.SetActive(isSelected);
    }

    public Coroutine SubSystemStartCoroutine(IEnumerator routine)
    {
        return StartCoroutine(routine);
    }
}

public interface ICoroutineHandler
{
    Coroutine SubSystemStartCoroutine(IEnumerator routine);
}
