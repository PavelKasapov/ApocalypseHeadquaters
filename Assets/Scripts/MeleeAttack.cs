using System.Collections;
using UnityEngine;
using Zenject;

public class MeleeAttack : Attack
{
    public override float MaxAttackRange { get; protected set; } = 0.75f;
    protected override float delayBeforeAttack { get; set; } = 0.5f;
    protected override float delayAfterAttack { get; set; } = 0.5f;

    public MeleeAttack(
        [Inject(Id = "ModelTransform")] Transform selfTransform,
        SightSystem sightSystem) : base(selfTransform, sightSystem) { }

    protected override IEnumerator SingleAttackRoutine()
    {
        CurrentStatusDuration = float.Epsilon;
        Status = AttackStatus.Attacking;
        
        attackTarget.targetInfo.Transform.gameObject
            .GetComponent<HitTaker>()
            .TakeDamage(new MeleeHit(attackTarget.targetInfo.Transform.position - selfTransform.position, 3f));
        
        yield return new WaitForFixedUpdate();
    }
}