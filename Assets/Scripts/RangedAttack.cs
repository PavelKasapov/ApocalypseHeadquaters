using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class RangedAttack : Attack
{
    private readonly BulletPool bulletPool;
    protected override float delayBeforeAttack { get; set; } = 1f;
    protected override float delayAfterAttack { get; set; } = 0.1f;
    private float delayBetweenShots = 0.1f;
    private float reloadTime = 3f;
    public override float MaxAttackRange { get; protected set; } = 5f;

    public int fullAmmo = 9;
    public int totalAmmo = 27;
    public int currentAmmo = 9;

    public RangedAttack(
        [Inject(Id = "ModelTransform")] Transform selfTransform,
        SightSystem sightSystem,
        BulletPool bulletPool) : base(selfTransform, sightSystem) 
    {
        this.bulletPool = bulletPool;
    }

    public override bool CanAttack(Target target)
    {
        return base.CanAttack(target)
            && currentAmmo > 0;
    }

    protected override IEnumerator SingleAttackRoutine()
    {
        int burstCount = 3;
        var target = attackTarget;

        CurrentStatusDuration = Math.Min(burstCount, currentAmmo) * delayBetweenShots;
        Status = AttackStatus.Attacking;

        while (burstCount > 0 && currentAmmo > 0)
        {
            var selfPosition = selfTransform.position + selfTransform.up * 0.3f;

            bulletPool.Pool.Get().Launch(selfPosition, selfTransform.up);

            burstCount--;
            currentAmmo--;
            yield return new WaitForSeconds(delayBetweenShots);
        }
    }

    protected override IEnumerator AttackRoutine()
    {
        while (totalAmmo > 0 || currentAmmo > 0)
        {
            if (currentAmmo <= 0)
            {
                yield return ReloadRoutine();
            }

            yield return base.AttackRoutine();

            if (currentAmmo <= 0 && totalAmmo > 0)
            {
                yield return ReloadRoutine();
            }
        }
        CurrentStatusDuration = float.Epsilon;
        Status = AttackStatus.NotPossible;
    }

    private IEnumerator ReloadRoutine()
    {
        CurrentStatusDuration = reloadTime;
        Status = AttackStatus.Reload;

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = Math.Min(fullAmmo, totalAmmo);
        totalAmmo -= currentAmmo;
    }
}