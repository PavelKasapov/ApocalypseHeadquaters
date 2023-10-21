using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public abstract class Attack
{
    public abstract float MaxAttackRange { get; protected set; }
    protected abstract WaitForSeconds delayBeforeAttack { get; set; }
    protected abstract WaitForSeconds delayAfterAttack { get; set; }

    protected readonly SightSystem sightSystem;
    protected readonly Transform selfTransform;
    protected Target attackTarget;
    protected Coroutine attackCoroutine;
    private Coroutine tryAttackCoroutine;
    private AttackStatus _status;
    Action<AttackStatus> OnStatusChange = delegate { };
    public AttackStatus Status 
    {
        get => _status;
        set 
        {
            if (_status != value)
            {
                _status = value;
                Debug.Log($"CanAttack(attackTarget) {attackTarget.targetInfo.EntityType} {CanAttack(attackTarget)} {value} {tryAttackCoroutine != null} {attackCoroutine != null}");
                //OnStatusChange(value);
            }
        }
    }
    
    public Attack(
        Transform selfTransform,
        SightSystem sightSystem)
    {
        this.selfTransform = selfTransform;
        this.sightSystem = sightSystem;
    }

    public virtual bool CanAttack(Target target)
    {
        target.UpdateDistance();
        return target.Distance < MaxAttackRange && sightSystem.VisibleTargets.ContainsKey(target.targetInfo);
    }

    public virtual void ChangeTarget(Target target)
    {
        if (this.attackTarget == target 
            || !selfTransform.gameObject.activeInHierarchy) return;

        Stop();
        Perform(target);
    }

    public void Stop()
    {
        if (tryAttackCoroutine != null) sightSystem.StopCoroutine(tryAttackCoroutine);
        if (attackCoroutine != null) sightSystem.StopCoroutine(attackCoroutine);
    }

    public void Perform(Target target)
    {
        if (target == null ) return;

        attackTarget = target;
        tryAttackCoroutine = sightSystem.StartCoroutine(TryAttackRoutine());
    }

    protected virtual IEnumerator AttackRoutine()
    {
        var target = attackTarget;

        while (target == attackTarget && attackTarget != null)
        {
            Status = AttackStatus.Aiming;
            yield return delayBeforeAttack;

            Status = AttackStatus.Attacking;
            yield return SingleAttackRoutine();

            Status = AttackStatus.AttackCooldown;
            yield return delayAfterAttack;
        }
        attackCoroutine = null;
    }

    protected abstract IEnumerator SingleAttackRoutine();

    IEnumerator TryAttackRoutine()
    {
        while (attackTarget != null)
        {
            if (Status != AttackStatus.Attacking)
            {
                if (CanAttack(attackTarget))
                {
                    attackCoroutine ??= sightSystem.StartCoroutine(AttackRoutine());
                }
                else
                {
                    if (attackCoroutine != null)
                    {
                        sightSystem.StopCoroutine(attackCoroutine);
                        attackCoroutine = null;
                    }

                    Status = AttackStatus.NotPossible;
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
