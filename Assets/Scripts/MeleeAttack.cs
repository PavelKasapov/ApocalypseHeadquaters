using System.Collections;
using UnityEngine;
using Zenject;

public class MeleeAttack : Attack
{
    private MeleeHit meleeHit = new(10f);
    public override float MaxAttackRange { get; protected set; } = 0.75f;
    protected override WaitForSeconds delayBeforeAttack { get; set; } = new(2f);
    protected override WaitForSeconds delayAfterAttack { get; set; } = new(0.5f);

    public MeleeAttack(
        [Inject(Id = "ModelTransform")] Transform selfTransform,
        SightSystem sightSystem) : base(selfTransform, sightSystem) { }

    protected override IEnumerator SingleAttackRoutine()
    {
        attackTarget.targetInfo.Transform.gameObject.GetComponent<HitTaker>().TakeDamage(meleeHit);
        yield return new WaitForFixedUpdate();
    }
}