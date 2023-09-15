using UnityEngine;
using Zenject;

public class CharacterInstaller : MonoInstaller<CharacterInstaller>
{
    [SerializeField] private Character character;
    [SerializeField] private Transform characterTransform;
    [SerializeField] private Transform modelTransform;
    [SerializeField] private MovementSystem movementSystem;
    [SerializeField] private SightSystem sightSystem;
    public override void InstallBindings()
    {
        Container.BindInstance<Character>(character);
        Container.BindInstance<Transform>(characterTransform).WithId("MainTransform");
        Container.BindInstance<Transform>(modelTransform).WithId("ModelTransform");
        Container.BindInstance<MovementSystem>(movementSystem);
        Container.BindInstance<SightSystem>(sightSystem);
        Container.Bind<HpSystem>().AsSingle();
        Container.BindInterfacesTo<RangeAttack>().AsSingle().NonLazy();
    }
}