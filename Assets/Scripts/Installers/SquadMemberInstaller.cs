public class SquadMemberInstaller : CharacterInstaller
{
    public override void InstallBindings()
    {
        base.InstallBindings();
        Container.BindInterfacesAndSelfTo<SquadSight>().AsSingle();
        Container.BindInterfacesAndSelfTo<RangedAttack>().AsSingle().NonLazy();
    }
}