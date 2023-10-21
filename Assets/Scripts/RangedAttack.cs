using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Zenject;

public class RangedAttack : Attack
{
    private readonly BulletPool bulletPool;
    protected override WaitForSeconds delayBeforeAttack { get; set; } = new(1f);
    protected override WaitForSeconds delayAfterAttack { get; set; } = new(0.1f);
    public override float MaxAttackRange { get; protected set; } = 5f;

    public int fullAmmo = 9;
    public int totalAmmo = 27;
    public int currentAmmo = 9;

    public RangedAttack(
        [Inject(Id = "ModelTransform")] Transform selfTransform,
        SightSystem sightSystem,
        BulletPool bulletPool) : base(selfTransform,sightSystem) 
    {
        this.bulletPool = bulletPool;
    }

    public override bool CanAttack(Target target)
    {
        return base.CanAttack(target)
            && (currentAmmo > 0 || totalAmmo > 0);
            /*&& Physics2D.CircleCast(selfTransform.position + selfTransform.up * 0.4f, 0.1f, attackTarget.targetInfo.Transform.position)*/;
    }

    protected override IEnumerator SingleAttackRoutine()
    {
        int burstCount = 3;
        var target = attackTarget;
        while (burstCount > 0 && currentAmmo > 0)
        {
            var selfPosition = selfTransform.position + selfTransform.up * 0.3f;
            var targetPosition = target.targetInfo.Transform.position;

            bulletPool.Pool.Get().Launch(selfPosition, (targetPosition - selfPosition).normalized);

            burstCount--;
            currentAmmo--;
            yield return new WaitForSeconds(0.1f);
        }
        if (currentAmmo <= 0)
        {
            yield return ReloadRoutine();
        }
    }

    protected override IEnumerator AttackRoutine()
    {
        if (currentAmmo < 0)
        {
            yield return ReloadRoutine();
        }
        yield return base.AttackRoutine();
    }

    private IEnumerator ReloadRoutine()
    {
        Debug.Log("Reload");
        Status = AttackStatus.Reload;
        yield return new WaitForSeconds(3f);
        currentAmmo = Math.Min(fullAmmo, totalAmmo);
        totalAmmo -= currentAmmo;
        Debug.Log($"Reload Complete");
    }
}