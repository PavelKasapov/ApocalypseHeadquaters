using UnityEngine;
using Zenject;

public class CharacterInstaller : MonoInstaller<CharacterInstaller>
{
    [SerializeField] private Character character;
    [SerializeField] private Transform characterTransform;
    [SerializeField] private Transform modelTransform;
    [SerializeField] private MovementSystem movementSystem;
    [SerializeField] private SightSystem sightSystem;
    [SerializeField] private CombatSystem combatSystem;
    [SerializeField] private TargetAimDrawer targetAimDrawer;
    [SerializeField] private PathDrawer pathDrawer;
    [SerializeField] private BehaviorColorIndicator behaviorColorIndicator;
    public override void InstallBindings()
    {
        Container.BindInstance(character);
        Container.BindInstance(characterTransform).WithId("MainTransform");
        Container.BindInstance(modelTransform).WithId("ModelTransform");
        Container.BindInstance(movementSystem);
        Container.BindInstance(sightSystem);
        Container.BindInstance(targetAimDrawer);
        Container.BindInstance(pathDrawer);
        Container.BindInstance(behaviorColorIndicator);
        Container.Bind<HpSystem>().AsSingle();
        Container.BindInterfacesAndSelfTo<CombatSystem>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<RangedAttack>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<MeleeAttack>().AsSingle().NonLazy();
    }
}