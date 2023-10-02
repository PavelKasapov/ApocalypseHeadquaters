using UnityEngine;
using Zenject;

public abstract class Attack
{
    public abstract float AttackRange { get; protected set; }

    protected readonly SightSystem sightSystem;
    protected readonly Transform selfTransform;

    public Attack(
        [Inject(Id = "ModelTransform")] Transform selfTransform,
        SightSystem sightSystem)
    {
        this.selfTransform = selfTransform;
        this.sightSystem = sightSystem;
    }
}
