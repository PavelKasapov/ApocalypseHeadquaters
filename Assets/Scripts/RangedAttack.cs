using System.Collections;
using UnityEngine;
using Zenject;

public class RangedAttack : Attack//, IInitializable, IDisposable
{
    private readonly BulletPool bulletPool;
    private readonly TargetAimDrawer targetAimDrawer;
    //private readonly BehaviorColorIndicator behaviorColorIndicator;

    private Coroutine shootingCoroutine;
    private Target attackTarget;

    public override float AttackRange { get; protected set; } = 5f;

    public RangedAttack(
        [Inject(Id = "ModelTransform")] Transform selfTransform,
        SightSystem sightSystem,
        BulletPool bulletPool,
        TargetAimDrawer targetAimDrawer) : base(selfTransform,sightSystem) 
    {
        this.bulletPool = bulletPool;
        this.targetAimDrawer = targetAimDrawer;
    }

    IEnumerator ShootingRoutine(Target target)
    {
        attackTarget = target;
        
        while (target == attackTarget && attackTarget != null)
        {
            yield return new WaitForSeconds(1f);

            var selfPosition = selfTransform.position + selfTransform.up * 0.3f;
            var targetPosition = attackTarget.targetInfo.Transform.position;

            Debug.DrawRay(selfPosition,
                targetPosition - selfPosition,
                Color.yellow);

            bulletPool.Pool.Get().Launch(selfPosition, (targetPosition - selfPosition).normalized);
        }
    }


    public void SetAttackTarget(Target target)
    {
        if (!selfTransform.gameObject.activeInHierarchy 
            || this.attackTarget == target) return;
        
        if (shootingCoroutine != null) sightSystem.StopCoroutine(shootingCoroutine);

        shootingCoroutine = sightSystem.StartCoroutine(ShootingRoutine(target));
    }
}