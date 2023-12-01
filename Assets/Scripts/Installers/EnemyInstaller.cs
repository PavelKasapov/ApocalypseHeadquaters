using UnityEngine;
using Zenject;

public class EnemyInstaller : CharacterInstaller
{
    public override void InstallBindings()
    {
        base.InstallBindings();
        Container.BindInterfacesAndSelfTo<EnemyAI>().AsSingle().NonLazy();
    }
}