using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class CharacterInstaller : MonoInstaller<CharacterInstaller>
{
    [SerializeField] private Character character;
    [SerializeField] private Transform characterTransform;
    [SerializeField] private Transform modelTransform;
    [SerializeField] private Rigidbody2D characterRigidbody;
    [SerializeField] private MovementSystem movementSystem;
    [SerializeField] private SightSystem sightSystem;
    [SerializeField] private CombatSystem combatSystem;
    [SerializeField] private TargetAimDrawer targetAimDrawer;
    [SerializeField] private BehaviorColorIndicator behaviorColorIndicator;
    [SerializeField] private SelectorHandler selectorHandler;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private HitTaker hitTaker;
    public override void InstallBindings()
    {
        Container.BindInstance(character);
        Container.BindInstance(characterTransform).WithId("MainTransform");
        Container.BindInstance(modelTransform).WithId("ModelTransform");
        Container.BindInstance(characterRigidbody);
        Container.BindInstance(movementSystem);
        Container.BindInstance(sightSystem);
        Container.BindInstance(targetAimDrawer);
        Container.BindInstance(behaviorColorIndicator);
        Container.BindInstance(selectorHandler);
        Container.BindInstance(navMeshAgent);
        Container.BindInstance(hitTaker);
        Container.Bind<HpSystem>().AsSingle();
        Container.BindInterfacesAndSelfTo<CombatSystem>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<MeleeAttack>().AsSingle().NonLazy();
    }
}